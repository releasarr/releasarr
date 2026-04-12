using System;
using System.Collections.Generic;
using System.Net;
using FluentValidation.Results;
using NLog;
using NzbDrone.Common.Http;
using NzbDrone.Common.Serializer;

namespace NzbDrone.Core.ArrClients.Sonarr
{
    public interface ISonarrProxy
    {
        List<SonarrSeries> GetAllSeries(SonarrSettings settings);
        SonarrSeries GetSeriesByTvdbId(int tvdbId, SonarrSettings settings);
        List<SonarrQueueItem> GetQueue(SonarrSettings settings);
        ValidationFailure Test(SonarrSettings settings);
    }

    public class SonarrProxy : ISonarrProxy
    {
        private readonly IHttpClient _httpClient;
        private readonly Logger _logger;

        public SonarrProxy(IHttpClient httpClient, Logger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public List<SonarrSeries> GetAllSeries(SonarrSettings settings)
        {
            var request = BuildRequest(settings, "/api/v3/series");
            return _httpClient.Get<List<SonarrSeries>>(request).Resource;
        }

        public SonarrSeries GetSeriesByTvdbId(int tvdbId, SonarrSettings settings)
        {
            var allSeries = GetAllSeries(settings);
            return allSeries?.Find(s => s.TvdbId == tvdbId);
        }

        public List<SonarrQueueItem> GetQueue(SonarrSettings settings)
        {
            var request = BuildRequest(settings, "/api/v3/queue?pageSize=100&includeUnknownSeriesItems=false");
            var response = _httpClient.Get<SonarrQueueResponse>(request);
            return response?.Resource?.Records ?? new List<SonarrQueueItem>();
        }

        public ValidationFailure Test(SonarrSettings settings)
        {
            try
            {
                var request = BuildRequest(settings, "/api/v3/system/status");
                var response = _httpClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new ValidationFailure("ApiKey", "Invalid Sonarr API key");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to connect to Sonarr");
                return new ValidationFailure("BaseUrl", "Unable to connect to Sonarr: " + ex.Message);
            }

            return null;
        }

        private HttpRequest BuildRequest(SonarrSettings settings, string path)
        {
            var baseUrl = settings.BaseUrl.TrimEnd('/');
            var requestBuilder = new HttpRequestBuilder($"{baseUrl}{path}")
                .SetHeader("X-Api-Key", settings.ApiKey)
                .Accept(HttpAccept.Json);

            return requestBuilder.Build();
        }
    }

    // Sonarr API DTOs
    public class SonarrSeries
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int TvdbId { get; set; }
        public int? TmdbId { get; set; }
        public string ImdbId { get; set; }
        public bool Monitored { get; set; }
        public SonarrSeriesStatistics Statistics { get; set; }
        public string Status { get; set; }
    }

    public class SonarrSeriesStatistics
    {
        public int EpisodeFileCount { get; set; }
        public int EpisodeCount { get; set; }
        public int TotalEpisodeCount { get; set; }
        public decimal PercentOfEpisodes { get; set; }
    }

    public class SonarrQueueResponse
    {
        public List<SonarrQueueItem> Records { get; set; }
    }

    public class SonarrQueueItem
    {
        public int SeriesId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public decimal Sizeleft { get; set; }
        public string TimeleftStr { get; set; }
    }
}
