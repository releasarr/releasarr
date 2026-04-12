using System;
using System.Linq;
using NLog;
using NzbDrone.Core.ArrClients;
using NzbDrone.Core.ArrClients.Radarr;
using NzbDrone.Core.ArrClients.Sonarr;
using NzbDrone.Core.MediaServers;
using NzbDrone.Core.MediaServers.Plex;
using NzbDrone.Core.Messaging.Commands;
using NzbDrone.Core.TrackedContent;

namespace NzbDrone.Core.Monitoring
{
    public class WatchlistSyncService : IExecute<WatchlistSyncCommand>
    {
        private readonly IMediaServerFactory _mediaServerFactory;
        private readonly IArrClientFactory _arrClientFactory;
        private readonly ITrackedItemService _trackedItemService;
        private readonly Logger _logger;

        public WatchlistSyncService(IMediaServerFactory mediaServerFactory,
                                     IArrClientFactory arrClientFactory,
                                     ITrackedItemService trackedItemService,
                                     Logger logger)
        {
            _mediaServerFactory = mediaServerFactory;
            _arrClientFactory = arrClientFactory;
            _trackedItemService = trackedItemService;
            _logger = logger;
        }

        public void Execute(WatchlistSyncCommand message)
        {
            var mediaServers = _mediaServerFactory.Enabled();

            foreach (var server in mediaServers)
            {
                if (server is PlexServer plexServer)
                {
                    SyncPlexWatchlist(plexServer);
                }
            }
        }

        private void SyncPlexWatchlist(PlexServer plexServer)
        {
            _logger.Info("Syncing watchlist from Plex server: {0}", plexServer.Definition.Name);

            var items = plexServer.GetWatchlistItems();
            _logger.Debug("Found {0} items in Plex watchlist/playlist", items.Count);

            var arrClients = _arrClientFactory.Enabled();

            foreach (var item in items)
            {
                try
                {
                    // Check if already tracked
                    if (item.PlexGuid != null)
                    {
                        var existing = _trackedItemService.FindByPlexGuid(item.PlexGuid);
                        if (existing != null)
                        {
                            continue;
                        }
                    }

                    var contentType = item.Type == "movie" ? ContentType.Movie : ContentType.Series;

                    var trackedItem = new TrackedItem
                    {
                        Title = item.Title,
                        ContentType = contentType,
                        PlexGuid = item.PlexGuid,
                        TmdbId = item.TmdbId,
                        TvdbId = item.TvdbId,
                        ImdbId = item.ImdbId,
                        MediaServerId = plexServer.Definition.Id,
                        Year = item.Year,
                        PosterUrl = item.PosterUrl
                    };

                    // Try to match to Sonarr/Radarr
                    TryMatchToArrClient(trackedItem, arrClients);

                    _trackedItemService.Add(trackedItem);
                    _logger.Info("Added tracked item: {0} ({1})", item.Title, contentType);
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to process watchlist item: {0}", item.Title);
                }
            }
        }

        private void TryMatchToArrClient(TrackedItem item, System.Collections.Generic.List<IArrClient> arrClients)
        {
            foreach (var client in arrClients)
            {
                try
                {
                    if (item.ContentType == ContentType.Series && client is SonarrClient sonarr && item.TvdbId.HasValue)
                    {
                        var sonarrProxy = GetSonarrProxy(sonarr);
                        if (sonarrProxy != null)
                        {
                            var series = sonarrProxy.GetSeriesByTvdbId(item.TvdbId.Value, (SonarrSettings)sonarr.Definition.Settings);
                            if (series != null)
                            {
                                item.ArrClientId = sonarr.Definition.Id;
                                item.ArrItemId = series.Id;
                                item.Status = TrackedItemStatus.Monitored;
                                _logger.Debug("Matched {0} to Sonarr series ID {1}", item.Title, series.Id);
                                return;
                            }
                        }
                    }
                    else if (item.ContentType == ContentType.Movie && client is RadarrClient radarr && item.TmdbId.HasValue)
                    {
                        var radarrProxy = GetRadarrProxy(radarr);
                        if (radarrProxy != null)
                        {
                            var movie = radarrProxy.GetMovieByTmdbId(item.TmdbId.Value, (RadarrSettings)radarr.Definition.Settings);
                            if (movie != null)
                            {
                                item.ArrClientId = radarr.Definition.Id;
                                item.ArrItemId = movie.Id;
                                item.Status = movie.HasFile ? TrackedItemStatus.Available : TrackedItemStatus.Monitored;
                                _logger.Debug("Matched {0} to Radarr movie ID {1}", item.Title, movie.Id);
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to match {0} against arr client {1}", item.Title, client.Definition.Name);
                }
            }
        }

        // Helper to get the proxy from the DI-injected client
        // The proxy is injected into the client via constructor, we access it via reflection or a new interface method
        // For simplicity, we create new proxy instances - the NzbDrone pattern uses field access
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
    }
}
