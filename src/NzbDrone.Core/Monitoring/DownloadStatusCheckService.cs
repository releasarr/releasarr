using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Core.ArrClients;
using NzbDrone.Core.ArrClients.Radarr;
using NzbDrone.Core.ArrClients.Sonarr;
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
        private readonly Logger _logger;

        public DownloadStatusCheckService(ITrackedItemService trackedItemService,
                                           IArrClientFactory arrClientFactory,
                                           IEventAggregator eventAggregator,
                                           Logger logger)
        {
            _trackedItemService = trackedItemService;
            _arrClientFactory = arrClientFactory;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public void Execute(DownloadStatusCheckCommand message)
        {
            var pendingItems = _trackedItemService.GetByStatuses(
                TrackedItemStatus.Watchlisted,
                TrackedItemStatus.Monitored,
                TrackedItemStatus.Downloading);

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

                    CheckItemStatus(item, client);
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to check status for: {0}", item.Title);
                }
            }
        }

        private void CheckItemStatus(TrackedItem item, IArrClient client)
        {
            var previousStatus = item.Status;
            ContentAvailableMessage contentMessage = null;

            if (item.ContentType == ContentType.Movie && client is RadarrClient radarr)
            {
                contentMessage = CheckRadarrStatus(item, radarr);
            }
            else if (item.ContentType == ContentType.Series && client is SonarrClient sonarr)
            {
                contentMessage = CheckSonarrStatus(item, sonarr);
            }

            // If status changed to Available, fire notification event
            if (previousStatus != TrackedItemStatus.Available && item.Status == TrackedItemStatus.Available)
            {
                if (contentMessage == null)
                {
                    contentMessage = BuildBasicMessage(item);
                }

                _logger.Info("Now available: {0}", contentMessage.Message);

                _eventAggregator.PublishEvent(new ContentAvailableEvent(contentMessage));
                _trackedItemService.UpdateStatus(item.Id, TrackedItemStatus.Notified);
            }
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

        private ContentAvailableMessage CheckRadarrStatus(TrackedItem item, RadarrClient radarr)
        {
            var settings = (RadarrSettings)radarr.Definition.Settings;
            var field = typeof(RadarrClient).GetField("_proxy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var proxy = field?.GetValue(radarr) as IRadarrProxy;

            if (proxy == null)
            {
                return null;
            }

            // Check queue for downloading status
            var queue = proxy.GetQueue(settings);
            var inQueue = queue.Any(q => q.MovieId == item.ArrItemId.Value);

            if (inQueue && item.Status != TrackedItemStatus.Downloading)
            {
                _trackedItemService.UpdateStatus(item.Id, TrackedItemStatus.Downloading);
                item.Status = TrackedItemStatus.Downloading;
                return null;
            }

            // Check if movie has file (available)
            var movie = proxy.GetMovieByTmdbId(item.TmdbId ?? 0, settings);
            if (movie != null && movie.HasFile)
            {
                _trackedItemService.UpdateStatus(item.Id, TrackedItemStatus.Available);
                item.Status = TrackedItemStatus.Available;

                var msg = BuildBasicMessage(item);
                msg.Overview = movie.Overview;
                msg.Runtime = movie.Runtime > 0 ? movie.Runtime : null;
                msg.Message = $"{movie.Title} ({movie.Year}) is now available!";

                return msg;
            }

            return null;
        }

        private ContentAvailableMessage CheckSonarrStatus(TrackedItem item, SonarrClient sonarr)
        {
            var settings = (SonarrSettings)sonarr.Definition.Settings;
            var field = typeof(SonarrClient).GetField("_proxy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var proxy = field?.GetValue(sonarr) as ISonarrProxy;

            if (proxy == null)
            {
                return null;
            }

            // Check queue for downloading status
            var queue = proxy.GetQueue(settings);
            var inQueue = queue.Any(q => q.SeriesId == item.ArrItemId.Value);

            if (inQueue && item.Status != TrackedItemStatus.Downloading)
            {
                _trackedItemService.UpdateStatus(item.Id, TrackedItemStatus.Downloading);
                item.Status = TrackedItemStatus.Downloading;
                return null;
            }

            // Check if series has episodes (available)
            var series = proxy.GetSeriesByTvdbId(item.TvdbId ?? 0, settings);
            if (series?.Statistics != null && series.Statistics.EpisodeFileCount > 0)
            {
                _trackedItemService.UpdateStatus(item.Id, TrackedItemStatus.Available);
                item.Status = TrackedItemStatus.Available;

                var msg = BuildBasicMessage(item);
                msg.Overview = series.Overview;
                msg.Runtime = series.Runtime;

                // Find the most recently available episode
                try
                {
                    var episodes = proxy.GetEpisodes(series.Id, settings);
                    _logger.Debug("Fetched {0} episodes for {1}, {2} with files",
                        episodes.Count, item.Title, episodes.Count(e => e.HasFile));

                    var latestWithFile = episodes
                        .Where(e => e.HasFile && e.SeasonNumber > 0)
                        .OrderByDescending(e => e.SeasonNumber)
                        .ThenByDescending(e => e.EpisodeNumber)
                        .FirstOrDefault();

                    if (latestWithFile != null)
                    {
                        msg.SeasonNumber = latestWithFile.SeasonNumber;
                        msg.EpisodeNumber = latestWithFile.EpisodeNumber;
                        msg.EpisodeTitle = latestWithFile.Title;
                        msg.Message = $"{series.Title} - S{latestWithFile.SeasonNumber:D2}E{latestWithFile.EpisodeNumber:D2} - {latestWithFile.Title} is now available!";
                    }
                    else
                    {
                        _logger.Debug("No episodes with files found for {0} (season > 0)", item.Title);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex, "Could not fetch episode details for {0}, using basic message", item.Title);
                }

                return msg;
            }

            return null;
        }
    }
}
