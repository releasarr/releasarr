namespace Releasarr.Api.V1.Dashboard
{
    public class DashboardResource
    {
        public int Watchlisted { get; set; }
        public int Monitored { get; set; }
        public int Downloading { get; set; }
        public int Available { get; set; }
        public int Notified { get; set; }
        public int Total { get; set; }
    }
}
