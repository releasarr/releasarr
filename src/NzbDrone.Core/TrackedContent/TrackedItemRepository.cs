using System.Collections.Generic;
using System.Linq;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;

namespace NzbDrone.Core.TrackedContent
{
    public interface ITrackedItemRepository : IBasicRepository<TrackedItem>
    {
        TrackedItem FindByPlexGuid(string plexGuid);
        TrackedItem FindByTvdbId(int tvdbId);
        TrackedItem FindByTmdbId(int tmdbId);
        List<TrackedItem> GetByStatus(TrackedItemStatus status);
        List<TrackedItem> GetByStatuses(params TrackedItemStatus[] statuses);
        List<TrackedItem> GetByMediaServerId(int mediaServerId);
    }

    public class TrackedItemRepository : BasicRepository<TrackedItem>, ITrackedItemRepository
    {
        public TrackedItemRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public TrackedItem FindByPlexGuid(string plexGuid)
        {
            return Query(x => x.PlexGuid == plexGuid).SingleOrDefault();
        }

        public TrackedItem FindByTvdbId(int tvdbId)
        {
            return Query(x => x.TvdbId == tvdbId).FirstOrDefault();
        }

        public TrackedItem FindByTmdbId(int tmdbId)
        {
            return Query(x => x.TmdbId == tmdbId).FirstOrDefault();
        }

        public List<TrackedItem> GetByStatus(TrackedItemStatus status)
        {
            return Query(x => x.Status == status);
        }

        public List<TrackedItem> GetByStatuses(params TrackedItemStatus[] statuses)
        {
            return Query(x => statuses.Contains(x.Status));
        }

        public List<TrackedItem> GetByMediaServerId(int mediaServerId)
        {
            return Query(x => x.MediaServerId == mediaServerId);
        }
    }
}
