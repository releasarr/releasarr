using System;
using System.Collections.Generic;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.TrackedContent
{
    public class TrackedItem : ModelBase
    {
        public string Title { get; set; }
        public ContentType ContentType { get; set; }
        public string PlexGuid { get; set; }
        public int? TmdbId { get; set; }
        public int? TvdbId { get; set; }
        public string ImdbId { get; set; }
        public int? ArrClientId { get; set; }
        public int? ArrItemId { get; set; }
        public TrackedItemStatus Status { get; set; }
        public int MediaServerId { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime? AvailableAt { get; set; }
        public DateTime? NotifiedAt { get; set; }
        public string Metadata { get; set; }
        public int? Year { get; set; }
        public string PosterUrl { get; set; }
    }
}
