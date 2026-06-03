using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Core.ArrClients;
using NzbDrone.Core.ArrClients.Radarr;
using NzbDrone.Core.ArrClients.Sonarr;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Messaging.Commands;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Notifications;
using NzbDrone.Core.TrackedContent;
using ContentType = NzbDrone.Core.TrackedContent.ContentType;

namespace NzbDrone.Core.Monitoring
{
    public class DownloadStatusCheckService : IExecute<DownloadStatusCheckCommand>
    {
        private readonly ITrackedItemService _trackedItemService;
        private readonly IArrClientFactory _arrClientFactory;
        private readonly IEventAggregator _eventAggregator;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public DownloadStatusCheckService(ITrackedItemService trackedItemService,
                                           IArrClientFactory arrClientFactory,
                                           IEventAggregator eventAggregator,
                                           IConfigService configService,
                                           Logger logger)
        {
            _trackedItemService = trackedItemService;
            _arrClientFactory = arrClientFactory;
            _eventAggregator = eventAggregator;
            _configService = configService;
            _logger = logger;
        }

        public void Execute(DownloadStatusCheckCommand message)
        {
            var pendingItems = _trackedItemService.GetByStatuses(
                TrackedItemStatus.Watchlisted,
                TrackedItemStatus.Monitored,
                TrackedItemStatus.Downloading,
                TrackedItemStatus.Available);

            if (!pendingItems.Any())
            {
                _logger.Debug("No pending tracked items to check");
                return;
            }

            var arrClients = _arrClientFactory.Enabled();
            var arrClientMap = new Dictionary<int, IArrClient>();
            foreach (var client in arrClients)
            {
                arrClientMap[client.Definition.Id] = client;
            }

            // The library and queue are fetched once per arr client per run and reused across
            // every tracked item. Fetching them per item previously meant pulling the entire
            // Sonarr/Radarr library (and queue) hundreds of times each cycle, which overloaded
            // the arr APIs and caused HTTP timeouts that silently skipped items (missed
            // notifications). Build the context lazily so we only hit clients that are actually
            // referenced by pending items.
            var sonarrContexts = new Dictionary<int, SonarrRunContext>();
            var radarrContexts = new Dictionary<int, RadarrRunContext>();

            foreach (var item in pendingItems)
            {
                try
                {
                    if (!item.ArrClientId.HasValue || !item.ArrItemId.HasValue)
                    {
                        continue;
                    }

                    if (!arrClientMap.TryGetValue(item.ArrClientId.Value, out var client))
                    {
                        continue;
                    }

                    if (item.ContentType == ContentType.Movie && client is RadarrClient radarr)
                    {
                        var context = GetOrBuildRadarrContext(radarrContexts, radarr);
                        if (context != null)
                        {
                            CheckRadarrStatus(item, context);
                        }
                    }
                    else if (item.ContentType == ContentType.Series && client is SonarrClient sonarr)
                    {
                        var context = GetOrBuildSonarrContext(sonarrContexts, sonarr);
                        if (context != null)
                        {
                            CheckSonarrStatus(item, context);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to check status for: {0}", item.Title);
                }
            }
        }

        private SonarrRunContext GetOrBuildSonarrContext(Dictionary<int, SonarrRunContext> cache, SonarrClient client)
        {
            if (cache.TryGetValue(client.Definition.Id, out var existing))
            {
                return existing;
            }

            SonarrRunContext context = null;

            try
            {
                var proxy = GetSonarrProxy(client);
                if (proxy != null)
                {
                    var settings = (SonarrSettings)client.Definition.Settings;
                    var seriesByTvdbId = new Dictionary<int, SonarrSeries>();

                    foreach (var series in proxy.GetAllSeries(settings) ?? new List<SonarrSeries>())
                    {
                        seriesByTvdbId[series.TvdbId] = series;
                    }

                    context = new SonarrRunContext
                    {
                        Proxy = proxy,
                        Settings = settings,
                        Queue = proxy.GetQueue(settings) ?? new List<SonarrQueueItem>(),
                        SeriesByTvdbId = seriesByTvdbId
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Failed to load library from Sonarr client: {0}", client.Definition.Name);
            }

            // Cache the result (even when null) so a failing client is not retried for every item.
            cache[client.Definition.Id] = context;
            return context;
        }

        private RadarrRunContext GetOrBuildRadarrContext(Dictionary<int, RadarrRunContext> cache, RadarrClient client)
        {
            if (cache.TryGetValue(client.Definition.Id, out var existing))
            {
                return existing;
            }

            RadarrRunContext context = null;

            try
            {
                var proxy = GetRadarrProxy(client);
                if (proxy != null)
                {
                    var settings = (RadarrSettings)client.Definition.Settings;
                    var moviesByTmdbId = new Dictionary<int, RadarrMovie>();

                    foreach (var movie in proxy.GetAllMovies(settings) ?? new List<RadarrMovie>())
                    {
                        moviesByTmdbId[movie.TmdbId] = movie;
                    }

                    context = new RadarrRunContext
                    {
                        Proxy = proxy,
                        Settings = settings,
                        Queue = proxy.GetQueue(settings) ?? new List<RadarrQueueItem>(),
                        MoviesByTmdbId = moviesByTmdbId
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Failed to load library from Radarr client: {0}", client.Definition.Name);
            }

            cache[client.Definition.Id] = context;
            return context;
        }

        private ContentAvailableMessage BuildBasicMessage(TrackedItem item)
        {
            return new ContentAvailableMessage
            {
                Title = item.Title,
                ContentType = item.ContentType,
                Message = $"{item.Title} ({item.Year}) is now available!",
                TrackedItem = item,
                Year = item.Year,
                PosterUrl = item.PosterUrl,
                TmdbUrl = item.TmdbId.HasValue ? $"https://www.themoviedb.org/{(item.ContentType == ContentType.Movie ? "movie" : "tv")}/{item.TmdbId}" : null,
                TvdbUrl = item.TvdbId.HasValue ? $"https://www.thetvdb.com/dereferrer/series/{item.TvdbId}" : null,
                ImdbUrl = item.ImdbId != null ? $"https://www.imdb.com/title/{item.ImdbId}/" : null,
            };
        }

        private void CheckRadarrStatus(TrackedItem item, RadarrRunContext context)
        {
            var metadata = item.GetMetadata();

            // Check queue for downloading status
            var queueItem = context.Queue.FirstOrDefault(q => q.MovieId == item.ArrItemId.Value);

            if (queueItem != null)
            {
                metadata.QueueStatus = queueItem.Status;
                metadata.QueueTimeleft = queueItem.Timeleft;

                if (item.Status != TrackedItemStatus.Downloading)
                {
                    _trackedItemService.UpdateStatus(item.Id, TrackedItemStatus.Downloading);
                    item.Status = TrackedItemStatus.Downloading;
                }
            }
            else
            {
                metadata.QueueStatus = null;
                metadata.QueueTimeleft = null;
            }

            // Check if movie has file (available)
            var movie = context.MoviesByTmdbId.TryGetValue(item.TmdbId ?? 0, out var matchedMovie) ? matchedMovie : null;
            if (movie != null)
            {
                metadata.Studio = movie.Studio;

                if (movie.HasFile && item.Status != TrackedItemStatus.Notified)
                {
                    _trackedItemService.UpdateStatus(item.Id, TrackedItemStatus.Available);
                    item.Status = TrackedItemStatus.Available;

                    var msg = BuildBasicMessage(item);
                    msg.Overview = movie.Overview;
                    msg.Runtime = movie.Runtime > 0 ? movie.Runtime : null;
                    msg.Message = $"{movie.Title} ({movie.Year}) is now available!";

                    _logger.Info("Now available: {0}", msg.Message);
                    _eventAggregator.PublishEvent(new ContentAvailableEvent(msg));

                    _trackedItemService.UpdateStatus(item.Id, TrackedItemStatus.Notified);
                    item.Status = TrackedItemStatus.Notified;
                }
            }

            item.SetMetadata(metadata);
            _trackedItemService.Update(item);
        }

        private void CheckSonarrStatus(TrackedItem item, SonarrRunContext context)
        {
            var metadata = item.GetMetadata();

            // Check queue for downloading status
            var queueItem = context.Queue.FirstOrDefault(q => q.SeriesId == item.ArrItemId.Value);

            if (queueItem != null)
            {
                metadata.QueueStatus = queueItem.Status;
                metadata.QueueTimeleft = queueItem.TimeleftStr;

                if (item.Status != TrackedItemStatus.Downloading)
                {
                    _trackedItemService.UpdateStatus(item.Id, TrackedItemStatus.Downloading);
                    item.Status = TrackedItemStatus.Downloading;
                }
            }
            else
            {
                metadata.QueueStatus = null;
                metadata.QueueTimeleft = null;
            }

            // Check episodes for availability + per-episode notifications
            var series = context.SeriesByTvdbId.TryGetValue(item.TvdbId ?? 0, out var matchedSeries) ? matchedSeries : null;
            if (series == null)
            {
                item.SetMetadata(metadata);
                _trackedItemService.Update(item);
                return;
            }

            // Populate enrichment metadata from series
            metadata.SeriesStatus = series.Status;
            metadata.Network = series.Network;

            if (series.Statistics != null)
            {
                metadata.EpisodeFileCount = series.Statistics.EpisodeFileCount;
                metadata.TotalEpisodeCount = series.Statistics.EpisodeCount;
            }

            if (series.Statistics == null || series.Statistics.EpisodeFileCount == 0)
            {
                item.SetMetadata(metadata);
                _trackedItemService.Update(item);
                return;
            }

            // Update to Available if not already
            if (item.Status != TrackedItemStatus.Available)
            {
                _trackedItemService.UpdateStatus(item.Id, TrackedItemStatus.Available);
                item.Status = TrackedItemStatus.Available;
            }

            // Per-episode notification logic
            try
            {
                var episodes = context.Proxy.GetEpisodes(series.Id, context.Settings);

                // Populate air date metadata
                var allEpisodes = episodes.Where(e => e.SeasonNumber > 0).ToList();

                var nextUnaired = allEpisodes
                    .Where(e => !e.HasFile && DateTime.TryParse(e.AirDateUtc, out var d) && d > DateTime.UtcNow)
                    .OrderBy(e => e.SeasonNumber)
                    .ThenBy(e => e.EpisodeNumber)
                    .FirstOrDefault();

                if (nextUnaired != null)
                {
                    metadata.NextEpisodeAirDateUtc = nextUnaired.AirDateUtc;
                }

                var lastAired = allEpisodes
                    .Where(e => e.HasFile)
                    .OrderByDescending(e => e.SeasonNumber)
                    .ThenByDescending(e => e.EpisodeNumber)
                    .FirstOrDefault();

                if (lastAired != null)
                {
                    metadata.LastEpisodeAirDateUtc = lastAired.AirDateUtc;
                }

                var mode = _configService.EpisodeNotificationMode;

                var episodesWithFiles = allEpisodes
                    .Where(e => e.HasFile)
                    .ToList();

                var episodesToNotify = GetEpisodesToNotify(episodesWithFiles, metadata, item.AddedAt, mode);

                foreach (var episode in episodesToNotify)
                {
                    var msg = BuildBasicMessage(item);
                    msg.Overview = series.Overview;
                    msg.Runtime = series.Runtime;
                    msg.SeasonNumber = episode.SeasonNumber;
                    msg.EpisodeNumber = episode.EpisodeNumber;
                    msg.EpisodeTitle = episode.Title;
                    msg.Message = $"{series.Title} - S{episode.SeasonNumber:D2}E{episode.EpisodeNumber:D2} - {episode.Title} is now available!";

                    _logger.Info("Now available: {0}", msg.Message);
                    _eventAggregator.PublishEvent(new ContentAvailableEvent(msg));

                    metadata.NotifiedEpisodeIds.Add(episode.Id);
                }

                metadata.LastKnownEpisodeFileCount = episodesWithFiles.Count;
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Could not fetch episode details for {0}", item.Title);
            }

            item.SetMetadata(metadata);
            _trackedItemService.Update(item);
        }

        private List<SonarrEpisode> GetEpisodesToNotify(
            List<SonarrEpisode> episodesWithFiles,
            TrackedItemMetadata metadata,
            DateTime addedAt,
            EpisodeNotificationMode mode)
        {
            var alreadyNotified = new HashSet<int>(metadata.NotifiedEpisodeIds);

            var candidates = episodesWithFiles
                .Where(e => !alreadyNotified.Contains(e.Id))
                .ToList();

            switch (mode)
            {
                case EpisodeNotificationMode.NewEpisodesOnly:
                    // Only episodes that aired after the item was added to Releasarr
                    return candidates
                        .Where(e => DateTime.TryParse(e.AirDateUtc, out var airDate) && airDate >= addedAt)
                        .OrderBy(e => e.SeasonNumber)
                        .ThenBy(e => e.EpisodeNumber)
                        .ToList();

                case EpisodeNotificationMode.AllNewFiles:
                    // Any newly detected file since last check
                    var previousCount = metadata.LastKnownEpisodeFileCount ?? 0;
                    if (episodesWithFiles.Count <= previousCount)
                    {
                        return new List<SonarrEpisode>();
                    }

                    // Return un-notified episodes (new files detected)
                    return candidates
                        .OrderByDescending(e => e.SeasonNumber)
                        .ThenByDescending(e => e.EpisodeNumber)
                        .Take(episodesWithFiles.Count - previousCount)
                        .OrderBy(e => e.SeasonNumber)
                        .ThenBy(e => e.EpisodeNumber)
                        .ToList();

                case EpisodeNotificationMode.AllEpisodes:
                    // First detection of any episode with a file
                    return candidates
                        .OrderBy(e => e.SeasonNumber)
                        .ThenBy(e => e.EpisodeNumber)
                        .ToList();

                default:
                    return new List<SonarrEpisode>();
            }
        }

        private ISonarrProxy GetSonarrProxy(SonarrClient client)
        {
            var field = typeof(SonarrClient).GetField("_proxy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(client) as ISonarrProxy;
        }

        private IRadarrProxy GetRadarrProxy(RadarrClient client)
        {
            var field = typeof(RadarrClient).GetField("_proxy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(client) as IRadarrProxy;
        }

        private sealed class SonarrRunContext
        {
            public ISonarrProxy Proxy { get; set; }
            public SonarrSettings Settings { get; set; }
            public List<SonarrQueueItem> Queue { get; set; }
            public Dictionary<int, SonarrSeries> SeriesByTvdbId { get; set; }
        }

        private sealed class RadarrRunContext
        {
            public IRadarrProxy Proxy { get; set; }
            public RadarrSettings Settings { get; set; }
            public List<RadarrQueueItem> Queue { get; set; }
            public Dictionary<int, RadarrMovie> MoviesByTmdbId { get; set; }
        }
    }
}
