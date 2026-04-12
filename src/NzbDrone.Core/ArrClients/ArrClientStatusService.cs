using System;
using NLog;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.ThingiProvider.Status;

namespace NzbDrone.Core.ArrClients
{
    public interface IArrClientStatusService : IProviderStatusServiceBase<ArrClientStatus>
    {
    }

    public class ArrClientStatusService : ProviderStatusServiceBase<IArrClient, ArrClientStatus>, IArrClientStatusService
    {
        public ArrClientStatusService(IArrClientStatusRepository providerStatusRepository, IEventAggregator eventAggregator, Logger logger)
            : base(providerStatusRepository, eventAggregator, logger)
        {
        }
    }
}
