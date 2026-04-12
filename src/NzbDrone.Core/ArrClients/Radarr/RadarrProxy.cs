using System;
using System.Collections.Generic;
using System.Net;
using FluentValidation.Results;
using NLog;
using NzbDrone.Common.Http;

namespace NzbDrone.Core.ArrClients.Radarr
{
    public interface IRadarrProxy
    {
        List<RadarrMovie> GetAllMovies(RadarrSettings settings);
        RadarrMovie GetMovieByTmdbId(int tmdbId, RadarrSettings settings);
        List<RadarrQueueItem> GetQueue(RadarrSettings settings);
        List<ArrTag> GetTags(RadarrSettings settings);
        List<RadarrMovie> LookupMovie(string term, RadarrSettings settings);
        ValidationFailure Test(RadarrSettings settings);
    }

    public class RadarrProxy : IRadarrProxy
    {
        private readonly IHttpClient _httpClient;
        private readonly Logger _logger;

        public RadarrProxy(IHttpClient httpClient, Logger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public List<RadarrMovie> GetAllMovies(RadarrSettings settings)
        {
            var request = BuildRequest(settings, "/api/v3/movie");
            return _httpClient.Get<List<RadarrMovie>>(request).Resource;
        }

        public RadarrMovie GetMovieByTmdbId(int tmdbId, RadarrSettings settings)
        {
            var allMovies = GetAllMovies(settings);
            return allMovies?.Find(m => m.TmdbId == tmdbId);
        }

        public List<RadarrQueueItem> GetQueue(RadarrSettings settings)
        {
            var request = BuildRequest(settings, "/api/v3/queue?pageSize=100&includeUnknownMovieItems=false");
            var response = _httpClient.Get<RadarrQueueResponse>(request);
            return response?.Resource?.Records ?? new List<RadarrQueueItem>();
        }

        public List<ArrTag> GetTags(RadarrSettings settings)
        {
            var request = BuildRequest(settings, "/api/v3/tag");
            var response = _httpClient.Get<List<ArrTag>>(request);
            return response?.Resource ?? new List<ArrTag>();
        }

        public List<RadarrMovie> LookupMovie(string term, RadarrSettings settings)
        {
            var request = BuildRequest(settings, $"/api/v3/movie/lookup?term={Uri.EscapeDataString(term)}");
            var response = _httpClient.Get<List<RadarrMovie>>(request);
            return response?.Resource ?? new List<RadarrMovie>();
        }

        public ValidationFailure Test(RadarrSettings settings)
        {
            try
            {
                var request = BuildRequest(settings, "/api/v3/system/status");
                var response = _httpClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new ValidationFailure("ApiKey", "Invalid Radarr API key");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to connect to Radarr");
                return new ValidationFailure("BaseUrl", "Unable to connect to Radarr: " + ex.Message);
            }

            return null;
        }

        private HttpRequest BuildRequest(RadarrSettings settings, string path)
        {
            var baseUrl = settings.BaseUrl.TrimEnd('/');
            var requestBuilder = new HttpRequestBuilder($"{baseUrl}{path}")
                .SetHeader("X-Api-Key", settings.ApiKey)
                .Accept(HttpAccept.Json);

            return requestBuilder.Build();
        }
    }

    // Radarr API DTOs
    public class RadarrMovie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int TmdbId { get; set; }
        public string ImdbId { get; set; }
        public int Year { get; set; }
        public bool HasFile { get; set; }
        public bool Monitored { get; set; }
        public string Status { get; set; }
        public string Overview { get; set; }
        public int Runtime { get; set; }
        public string Studio { get; set; }
        public List<int> Tags { get; set; } = new();
        public List<ArrImage> Images { get; set; } = new();
    }

    public class RadarrQueueResponse
    {
        public List<RadarrQueueItem> Records { get; set; }
    }

    public class RadarrQueueItem
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public decimal Sizeleft { get; set; }
        public string Timeleft { get; set; }
    }
}
