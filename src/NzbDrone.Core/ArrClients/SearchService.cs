using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Core.ArrClients.Radarr;
using NzbDrone.Core.ArrClients.Sonarr;
using NzbDrone.Core.MediaServers;
using NzbDrone.Core.MediaServers.Plex;

namespace NzbDrone.Core.ArrClients
{
    public interface ISearchService
    {
        List<SearchResult> Search(string query);
        void AddToWatchlist(int mediaServerId, int? tmdbId, int? tvdbId);
    }

    public class SearchService : ISearchService
    {
        private readonly IArrClientFactory _arrClientFactory;
        private readonly IMediaServerFactory _mediaServerFactory;
        private readonly Logger _logger;

        public SearchService(IArrClientFactory arrClientFactory,
                             IMediaServerFactory mediaServerFactory,
                             Logger logger)
        {
            _arrClientFactory = arrClientFactory;
            _mediaServerFactory = mediaServerFactory;
            _logger = logger;
        }

        public List<SearchResult> Search(string query)
        {
            var results = new List<SearchResult>();
            var seenTmdb = new HashSet<int>();
            var seenTvdb = new HashSet<int>();
            var arrClients = _arrClientFactory.Enabled();

            foreach (var client in arrClients)
            {
                try
                {
                    if (client is RadarrClient radarr)
                    {
                        var proxy = GetRadarrProxy(radarr);
                        if (proxy == null)
                        {
                            continue;
                        }

                        var settings = (RadarrSettings)radarr.Definition.Settings;
                        var movies = proxy.LookupMovie(query, settings);

                        foreach (var movie in movies)
                        {
                            if (seenTmdb.Contains(movie.TmdbId))
                            {
                                continue;
                            }

                            seenTmdb.Add(movie.TmdbId);

                            results.Add(new SearchResult
                            {
                                Title = movie.Title,
                                Type = "movie",
                                Year = movie.Year,
                                Overview = movie.Overview,
                                PosterUrl = movie.Images?.FirstOrDefault(i => i.CoverType == "poster")?.RemoteUrl,
                                TmdbId = movie.TmdbId,
                                ImdbId = movie.ImdbId,
                                Studio = movie.Studio,
                                Runtime = movie.Runtime > 0 ? movie.Runtime : null
                            });
                        }
                    }
                    else if (client is SonarrClient sonarr)
                    {
                        var proxy = GetSonarrProxy(sonarr);
                        if (proxy == null)
                        {
                            continue;
                        }

                        var settings = (SonarrSettings)sonarr.Definition.Settings;
                        var series = proxy.LookupSeries(query, settings);

                        foreach (var s in series)
                        {
                            if (seenTvdb.Contains(s.TvdbId))
                            {
                                continue;
                            }

                            seenTvdb.Add(s.TvdbId);

                            results.Add(new SearchResult
                            {
                                Title = s.Title,
                                Type = "series",
                                Year = s.Year,
                                Overview = s.Overview,
                                PosterUrl = s.Images?.FirstOrDefault(i => i.CoverType == "poster")?.RemoteUrl,
                                TvdbId = s.TvdbId,
                                TmdbId = s.TmdbId,
                                ImdbId = s.ImdbId,
                                Network = s.Network,
                                Runtime = s.Runtime
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to search {0}: {1}", client.Definition.Name, query);
                }
            }

            return results;
        }

        public void AddToWatchlist(int mediaServerId, int? tmdbId, int? tvdbId)
        {
            // Find the Plex ratingKey by searching Plex discover for the tmdb/tvdb ID
            // For now, we use the Plex discover search to find the ratingKey
            var servers = _mediaServerFactory.Enabled();
            var plexServer = servers.FirstOrDefault(s => s.Definition.Id == mediaServerId) as PlexServer;

            if (plexServer == null)
            {
                _logger.Warn("Media server {0} not found or not a Plex server", mediaServerId);
                return;
            }

            var proxy = GetPlexProxy(plexServer);
            if (proxy == null)
            {
                return;
            }

            // Search Plex discover to find the ratingKey for this content
            var searchTerm = tmdbId.HasValue ? $"tmdb-{tmdbId}" : $"tvdb-{tvdbId}";

            // We need to search Plex by the external ID - use the metadata endpoint
            var ratingKey = FindPlexRatingKey(proxy, plexServer.Settings, tmdbId, tvdbId);

            if (ratingKey != null)
            {
                proxy.AddToWatchlist(ratingKey, plexServer.Settings);
                _logger.Info("Added content to Plex watchlist (ratingKey: {0})", ratingKey);
            }
            else
            {
                _logger.Warn("Could not find Plex ratingKey for tmdbId={0} tvdbId={1}", tmdbId, tvdbId);
            }
        }

        private string FindPlexRatingKey(IPlexProxy proxy, PlexSettings settings, int? tmdbId, int? tvdbId)
        {
            // Search Plex discover using a GUID-based lookup
            // Plex discover supports searching by external source
            try
            {
                string guid = null;

                if (tmdbId.HasValue)
                {
                    guid = $"com.plexapp.agents.themoviedb://{tmdbId}";
                }
                else if (tvdbId.HasValue)
                {
                    guid = $"com.plexapp.agents.thetvdb://{tvdbId}";
                }

                if (guid == null)
                {
                    return null;
                }

                // Use the matches endpoint to find by external ID
                var httpClient = GetHttpClient();
                if (httpClient == null)
                {
                    return null;
                }

                var request = new NzbDrone.Common.Http.HttpRequestBuilder($"https://discover.provider.plex.tv/library/metadata/matches")
                    .AddQueryParam("X-Plex-Token", settings.AuthToken)
                    .AddQueryParam("type", tmdbId.HasValue ? "1" : "2")
                    .AddQueryParam("guid", tmdbId.HasValue ? $"tmdb://{tmdbId}" : $"tvdb://{tvdbId}")
                    .Accept(NzbDrone.Common.Http.HttpAccept.Json)
                    .Build();

                request.Headers.Add("X-Plex-Client-Identifier", "releasarr");

                var response = httpClient.Execute(request);
                var json = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                var metadata = json["MediaContainer"]?["Metadata"] as Newtonsoft.Json.Linq.JArray;

                return metadata?.FirstOrDefault()?.Value<string>("ratingKey");
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Could not find Plex ratingKey via metadata matches");
                return null;
            }
        }

        private NzbDrone.Common.Http.IHttpClient GetHttpClient()
        {
            // Access the IHttpClient from PlexProxy via reflection (same pattern used elsewhere)
            var servers = _mediaServerFactory.Enabled();
            var plexServer = servers.FirstOrDefault(s => s is PlexServer) as PlexServer;
            if (plexServer == null)
            {
                return null;
            }

            var proxy = GetPlexProxy(plexServer);
            if (proxy == null)
            {
                return null;
            }

            var field = typeof(PlexProxy).GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(proxy) as NzbDrone.Common.Http.IHttpClient;
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

        private IPlexProxy GetPlexProxy(PlexServer server)
        {
            var field = typeof(PlexServer).GetField("_proxy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(server) as IPlexProxy;
        }
    }
}
