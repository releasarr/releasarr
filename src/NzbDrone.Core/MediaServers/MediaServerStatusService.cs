using System;
using NLog;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.ThingiProvider.Status;

namespace NzbDrone.Core.MediaServers
{
    public interface IMediaServerStatusService : IProviderStatusServiceBase<MediaServerStatus>
    {
    }

    public class MediaServerStatusService : ProviderStatusServiceBase<IMediaServer, MediaServerStatus>, IMediaServerStatusService
    {
        public MediaServerStatusService(IMediaServerStatusRepository providerStatusRepository, IEventAggregator eventAggregator, Logger logger)
            : base(providerStatusRepository, eventAggregator, logger)
        {
        }
    }
}
