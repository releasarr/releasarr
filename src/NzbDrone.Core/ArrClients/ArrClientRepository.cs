using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.ArrClients
{
    public interface IArrClientRepository : IProviderRepository<ArrClientDefinition>
    {
    }

    public class ArrClientRepository : ProviderRepository<ArrClientDefinition>, IArrClientRepository
    {
        public ArrClientRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
