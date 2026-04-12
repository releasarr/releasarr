using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.TrackedContent;
using Releasarr.Http;

namespace Releasarr.Api.V1.TrackedContent
{
    [V1ApiController("trackedcontent")]
    public class TrackedItemController : Controller
    {
        private readonly ITrackedItemService _trackedItemService;

        public TrackedItemController(ITrackedItemService trackedItemService)
        {
            _trackedItemService = trackedItemService;
        }

        [HttpGet]
        [Produces("application/json")]
        public List<TrackedItemResource> GetAll([FromQuery] TrackedItemStatus? status = null)
        {
            if (status.HasValue)
            {
                return _trackedItemService.GetByStatus(status.Value)
                    .Select(x => x.ToResource())
                    .ToList();
            }

            return _trackedItemService.GetAll()
                .Select(x => x.ToResource())
                .ToList();
        }

        [HttpGet("{id:int}")]
        [Produces("application/json")]
        public TrackedItemResource GetById(int id)
        {
            return _trackedItemService.Get(id).ToResource();
        }

        [HttpDelete("{id:int}")]
        public object Delete(int id)
        {
            _trackedItemService.Delete(id);
            return new { };
        }
    }
}
