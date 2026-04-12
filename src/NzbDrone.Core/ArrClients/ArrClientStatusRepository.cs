using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.ThingiProvider.Status;

namespace NzbDrone.Core.ArrClients
{
    public interface IArrClientStatusRepository : IProviderStatusRepository<ArrClientStatus>
    {
    }

    public class ArrClientStatusRepository : ProviderStatusRepository<ArrClientStatus>, IArrClientStatusRepository
    {
        public ArrClientStatusRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
