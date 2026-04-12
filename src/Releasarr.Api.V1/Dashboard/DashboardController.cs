using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.TrackedContent;
using Releasarr.Http;

namespace Releasarr.Api.V1.Dashboard
{
    [V1ApiController]
    public class DashboardController : Controller
    {
        private readonly ITrackedItemService _trackedItemService;

        public DashboardController(ITrackedItemService trackedItemService)
        {
            _trackedItemService = trackedItemService;
        }

        [HttpGet]
        [Produces("application/json")]
        public DashboardResource GetDashboard()
        {
            var stats = _trackedItemService.GetDashboardStats();
            return new DashboardResource
            {
                Watchlisted = stats.Watchlisted,
                Monitored = stats.Monitored,
                Downloading = stats.Downloading,
                Available = stats.Available,
                Notified = stats.Notified,
                Total = stats.Total
            };
        }
    }
}
