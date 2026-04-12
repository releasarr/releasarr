using System;
using System.Collections.Generic;

namespace NzbDrone.Core.Notifications
{
    public class ReleaseInfo
    {
        public int IndexerId { get; set; }
        public string Title { get; set; }
        public string Indexer { get; set; }
        public long? Size { get; set; }
        public List<ReleaseCategory> Categories { get; set; } = new List<ReleaseCategory>();
        public List<string> Genres { get; set; } = new List<string>();
        public List<ReleaseFlag> IndexerFlags { get; set; } = new List<ReleaseFlag>();
        public DateTime PublishDate { get; set; }
        public string InfoUrl { get; set; }
    }

    public class ReleaseCategory
    {
        public string Name { get; set; }
    }

    public class ReleaseFlag
    {
        public string Name { get; set; }
    }
}
