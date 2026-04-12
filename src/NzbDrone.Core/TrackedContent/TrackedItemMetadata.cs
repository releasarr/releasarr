using System.Collections.Generic;
using Newtonsoft.Json;

namespace NzbDrone.Core.TrackedContent
{
    public class TrackedItemMetadata
    {
        public List<int> NotifiedEpisodeIds { get; set; } = new();
        public int? LastKnownEpisodeFileCount { get; set; }

        // Enrichment fields (Feature 4)
        public int? EpisodeFileCount { get; set; }
        public int? TotalEpisodeCount { get; set; }
        public string NextEpisodeAirDateUtc { get; set; }
        public string LastEpisodeAirDateUtc { get; set; }
        public string SeriesStatus { get; set; }
        public string Network { get; set; }
        public string Studio { get; set; }
        public string QueueStatus { get; set; }
        public string QueueTimeleft { get; set; }
    }

    public static class TrackedItemMetadataExtensions
    {
        public static TrackedItemMetadata GetMetadata(this TrackedItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Metadata))
            {
                return new TrackedItemMetadata();
            }

            try
            {
                return JsonConvert.DeserializeObject<TrackedItemMetadata>(item.Metadata) ?? new TrackedItemMetadata();
            }
            catch
            {
                return new TrackedItemMetadata();
            }
        }

        public static void SetMetadata(this TrackedItem item, TrackedItemMetadata metadata)
        {
            item.Metadata = JsonConvert.SerializeObject(metadata);
        }
    }
}
