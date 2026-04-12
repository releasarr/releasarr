using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation.Results;
using Newtonsoft.Json.Linq;
using NLog;
using NzbDrone.Common.Http;

namespace NzbDrone.Core.MediaServers.Plex
{
    public interface IPlexProxy
    {
        List<PlexWatchlistItem> GetWatchlist(PlexSettings settings);
        List<PlexWatchlistItem> GetPlaylistItems(string playlistName, PlexSettings settings);
        void AddToWatchlist(string ratingKey, PlexSettings settings);
        ValidationFailure Test(PlexSettings settings);
    }

    public class PlexProxy : IPlexProxy
    {
        private readonly IHttpClient _httpClient;
        private readonly Logger _logger;

        public PlexProxy(IHttpClient httpClient, Logger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public List<PlexWatchlistItem> GetWatchlist(PlexSettings settings)
        {
            var request = BuildWatchlistRequest(settings);
            var response = _httpClient.Execute(request);

            var json = JObject.Parse(response.Content);
            var metadata = json["MediaContainer"]?["Metadata"] as JArray;

            if (metadata == null)
            {
                return new List<PlexWatchlistItem>();
            }

            return metadata
                .Select(ParseWatchlistItem)
                .ToList();
        }

        public List<PlexWatchlistItem> GetPlaylistItems(string playlistName, PlexSettings settings)
        {
            // First get all playlists
            var playlistRequest = BuildServerRequest(settings, "/playlists");
            var playlistResponse = _httpClient.Get<PlexMediaContainer>(playlistRequest);

            if (playlistResponse?.Resource?.MediaContainer?.Metadata == null)
            {
                return new List<PlexWatchlistItem>();
            }

            var playlist = playlistResponse.Resource.MediaContainer.Metadata
                .FirstOrDefault(p => p.Title.Equals(playlistName, StringComparison.OrdinalIgnoreCase));

            if (playlist == null)
            {
                _logger.Warn("Playlist '{0}' not found on Plex server", playlistName);
                return new List<PlexWatchlistItem>();
            }

            // Get items from the playlist using JObject parsing (same guid/Guid case issue)
            var itemsRequest = BuildServerRequest(settings, playlist.Key);
            var itemsResponse = _httpClient.Execute(itemsRequest);
            var json = JObject.Parse(itemsResponse.Content);
            var metadata = json["MediaContainer"]?["Metadata"] as JArray;

            if (metadata == null)
            {
                return new List<PlexWatchlistItem>();
            }

            return metadata
                .Select(ParseWatchlistItem)
                .ToList();
        }

        public void AddToWatchlist(string ratingKey, PlexSettings settings)
        {
            var requestBuilder = new HttpRequestBuilder("https://discover.provider.plex.tv/actions/addToWatchlist")
                .AddQueryParam("ratingKey", ratingKey)
                .AddQueryParam("X-Plex-Token", settings.AuthToken);

            requestBuilder.Headers.Add("X-Plex-Client-Identifier", "releasarr");

            var request = requestBuilder.Build();
            request.Method = System.Net.Http.HttpMethod.Put;

            _httpClient.Execute(request);
        }

        public ValidationFailure Test(PlexSettings settings)
        {
            try
            {
                var request = BuildServerRequest(settings, "/identity");
                var response = _httpClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new ValidationFailure("AuthToken", "Invalid Plex auth token");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to connect to Plex server");
                return new ValidationFailure("ServerUrl", "Unable to connect to Plex server: " + ex.Message);
            }

            return null;
        }

        private HttpRequest BuildWatchlistRequest(PlexSettings settings)
        {
            var requestBuilder = new HttpRequestBuilder("https://discover.provider.plex.tv/library/sections/watchlist/all")
                .AddQueryParam("X-Plex-Token", settings.AuthToken)
                .AddQueryParam("includeGuids", "1")
                .AddQueryParam("X-Plex-Container-Start", "0")
                .AddQueryParam("X-Plex-Container-Size", "300")
                .Accept(HttpAccept.Json);

            requestBuilder.Headers.Add("X-Plex-Client-Identifier", "releasarr");

            return requestBuilder.Build();
        }

        private HttpRequest BuildServerRequest(PlexSettings settings, string path)
        {
            var baseUrl = settings.ServerUrl.TrimEnd('/');
            var requestBuilder = new HttpRequestBuilder($"{baseUrl}{path}")
                .AddQueryParam("X-Plex-Token", settings.AuthToken)
                .Accept(HttpAccept.Json);

            return requestBuilder.Build();
        }

        private PlexWatchlistItem ParseWatchlistItem(JToken token)
        {
            var item = new PlexWatchlistItem
            {
                Title = token.Value<string>("title"),
                Type = token.Value<string>("type"),
                Year = token.Value<int?>("year"),
                PlexGuid = token.Value<string>("guid"),
                PosterUrl = token.Value<string>("thumb")
            };

            // Parse external GUIDs from the "Guid" array (capital G, separate from "guid" string)
            var guids = token["Guid"] as JArray;
            if (guids != null)
            {
                foreach (var guid in guids)
                {
                    var id = guid.Value<string>("id");
                    if (id != null)
                    {
                        if (id.StartsWith("tmdb://") && int.TryParse(id.AsSpan(7), out var tmdbId))
                        {
                            item.TmdbId = tmdbId;
                        }
                        else if (id.StartsWith("tvdb://") && int.TryParse(id.AsSpan(7), out var tvdbId))
                        {
                            item.TvdbId = tvdbId;
                        }
                        else if (id.StartsWith("imdb://"))
                        {
                            item.ImdbId = id[7..];
                        }
                    }
                }
            }

            return item;
        }
    }

    // Plex API response DTOs (used for server-side endpoints like playlists)
    public class PlexMediaContainer
    {
        public PlexMediaContainerInner MediaContainer { get; set; }
    }

    public class PlexMediaContainerInner
    {
        public List<PlexMetadata> Metadata { get; set; }
    }

    public class PlexMetadata
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Key { get; set; }
    }
}
