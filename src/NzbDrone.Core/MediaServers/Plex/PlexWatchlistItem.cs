namespace NzbDrone.Core.MediaServers.Plex
{
    public class PlexWatchlistItem
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public int? Year { get; set; }
        public string PlexGuid { get; set; }
        public int? TmdbId { get; set; }
        public int? TvdbId { get; set; }
        public string ImdbId { get; set; }
        public string PosterUrl { get; set; }
    }
}
