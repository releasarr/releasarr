using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.MediaServers
{
    public interface IMediaServerRepository : IProviderRepository<MediaServerDefinition>
    {
    }

    public class MediaServerRepository : ProviderRepository<MediaServerDefinition>, IMediaServerRepository
    {
        public MediaServerRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
