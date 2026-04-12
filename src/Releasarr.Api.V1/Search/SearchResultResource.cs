using NzbDrone.Core.ArrClients;
using Releasarr.Http.REST;

namespace Releasarr.Api.V1.Search
{
    public class SearchResultResource : RestResource
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public int? Year { get; set; }
        public string Overview { get; set; }
        public string PosterUrl { get; set; }
        public int? TmdbId { get; set; }
        public int? TvdbId { get; set; }
        public string ImdbId { get; set; }
        public string Network { get; set; }
        public string Studio { get; set; }
        public int? Runtime { get; set; }
    }

    public class AddToWatchlistResource
    {
        public int MediaServerId { get; set; }
        public int? TmdbId { get; set; }
        public int? TvdbId { get; set; }
    }

    public static class SearchResultResourceMapper
    {
        public static SearchResultResource ToResource(this SearchResult model, int index)
        {
            return new SearchResultResource
            {
                Id = index,
                Title = model.Title,
                Type = model.Type,
                Year = model.Year,
                Overview = model.Overview,
                PosterUrl = model.PosterUrl,
                TmdbId = model.TmdbId,
                TvdbId = model.TvdbId,
                ImdbId = model.ImdbId,
                Network = model.Network,
                Studio = model.Studio,
                Runtime = model.Runtime
            };
        }
    }
}
