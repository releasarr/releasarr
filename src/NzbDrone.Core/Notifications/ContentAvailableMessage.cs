using NzbDrone.Core.TrackedContent;

namespace NzbDrone.Core.Notifications
{
    public class ContentAvailableMessage
    {
        public string Title { get; set; }
        public ContentType ContentType { get; set; }
        public string Message { get; set; }
        public TrackedItem TrackedItem { get; set; }

        // Rich metadata
        public string Overview { get; set; }
        public int? Year { get; set; }
        public string PosterUrl { get; set; }
        public int? Runtime { get; set; }
        public string Quality { get; set; }

        // Episode-specific (series only)
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
        public string EpisodeTitle { get; set; }

        // External links
        public string TvdbUrl { get; set; }
        public string TmdbUrl { get; set; }
        public string ImdbUrl { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
