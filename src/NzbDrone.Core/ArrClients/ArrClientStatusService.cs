using NLog;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.ThingiProvider.Status;

namespace NzbDrone.Core.ArrClients
{
    public interface IArrClientStatusService : IProviderStatusServiceBase<ArrClientStatus>
    {
    }

    public class ArrClientStatusService : ProviderStatusServiceBase<IArrClient, ArrClientStatus>, IArrClientStatusService
    {
        public ArrClientStatusService(IArrClientStatusRepository providerStatusRepository, IEventAggregator eventAggregator, IRuntimeInfo runtimeInfo, Logger logger)
            : base(providerStatusRepository, eventAggregator, runtimeInfo, logger)
        {
        }
    }
}
