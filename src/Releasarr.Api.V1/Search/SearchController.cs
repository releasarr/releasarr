using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.ArrClients;
using Releasarr.Http;

namespace Releasarr.Api.V1.Search
{
    [V1ApiController("search")]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        [Produces("application/json")]
        public List<SearchResultResource> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<SearchResultResource>();
            }

            return _searchService.Search(query)
                .Select((r, i) => r.ToResource(i + 1))
                .ToList();
        }

        [HttpPost("addtowatchlist")]
        [Produces("application/json")]
        public object AddToWatchlist([FromBody] AddToWatchlistResource resource)
        {
            _searchService.AddToWatchlist(resource.MediaServerId, resource.TmdbId, resource.TvdbId);
            return new { success = true };
        }
    }
}
