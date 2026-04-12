namespace NzbDrone.Core.ArrClients
{
    public class SearchResult
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
}
