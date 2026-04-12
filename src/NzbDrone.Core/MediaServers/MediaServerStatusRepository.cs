using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.ThingiProvider.Status;

namespace NzbDrone.Core.MediaServers
{
    public interface IMediaServerStatusRepository : IProviderStatusRepository<MediaServerStatus>
    {
    }

    public class MediaServerStatusRepository : ProviderStatusRepository<MediaServerStatus>, IMediaServerStatusRepository
    {
        public MediaServerStatusRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
