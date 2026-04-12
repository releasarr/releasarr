using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation.Results;
using NLog;
using NzbDrone.Common.Http;
using NzbDrone.Common.Serializer;

namespace NzbDrone.Core.MediaServers.Plex
{
    public interface IPlexProxy
    {
        List<PlexWatchlistItem> GetWatchlist(PlexSettings settings);
        List<PlexWatchlistItem> GetPlaylistItems(string playlistName, PlexSettings settings);
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
            var response = _httpClient.Get<PlexResponse<PlexMediaContainer>>(request);

            if (response?.Resource?.MediaContainer?.Metadata == null)
            {
                return new List<PlexWatchlistItem>();
            }

            return response.Resource.MediaContainer.Metadata
                .Select(MapToWatchlistItem)
                .ToList();
        }

        public List<PlexWatchlistItem> GetPlaylistItems(string playlistName, PlexSettings settings)
        {
            // First get all playlists
            var playlistRequest = BuildServerRequest(settings, "/playlists");
            var playlistResponse = _httpClient.Get<PlexResponse<PlexMediaContainer>>(playlistRequest);

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

            // Get items from the playlist
            var itemsRequest = BuildServerRequest(settings, playlist.Key);
            var itemsResponse = _httpClient.Get<PlexResponse<PlexMediaContainer>>(itemsRequest);

            if (itemsResponse?.Resource?.MediaContainer?.Metadata == null)
            {
                return new List<PlexWatchlistItem>();
            }

            return itemsResponse.Resource.MediaContainer.Metadata
                .Select(MapToWatchlistItem)
                .ToList();
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
            var requestBuilder = new HttpRequestBuilder("https://metadata.provider.plex.tv/library/sections/watchlist/all")
                .AddQueryParam("X-Plex-Token", settings.AuthToken)
                .Accept(HttpAccept.Json);

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

        private PlexWatchlistItem MapToWatchlistItem(PlexMetadata metadata)
        {
            var item = new PlexWatchlistItem
            {
                Title = metadata.Title,
                Type = metadata.Type,
                Year = metadata.Year,
                PlexGuid = metadata.Guid,
                PosterUrl = metadata.Thumb
            };

            // Parse external GUIDs (tmdb://, tvdb://, imdb://)
            if (metadata.Guids != null)
            {
                foreach (var guid in metadata.Guids)
                {
                    if (guid.Id != null)
                    {
                        if (guid.Id.StartsWith("tmdb://") && int.TryParse(guid.Id.Substring(7), out var tmdbId))
                        {
                            item.TmdbId = tmdbId;
                        }
                        else if (guid.Id.StartsWith("tvdb://") && int.TryParse(guid.Id.Substring(7), out var tvdbId))
                        {
                            item.TvdbId = tvdbId;
                        }
                        else if (guid.Id.StartsWith("imdb://"))
                        {
                            item.ImdbId = guid.Id.Substring(7);
                        }
                    }
                }
            }

            return item;
        }
    }

    // Plex API response DTOs
    public class PlexResponse<T>
    {
        public T Resource { get; set; }
    }

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
        public int? Year { get; set; }
        public string Guid { get; set; }
        public string Key { get; set; }
        public string Thumb { get; set; }
        public List<PlexGuid> Guids { get; set; }
    }

    public class PlexGuid
    {
        public string Id { get; set; }
    }
}
