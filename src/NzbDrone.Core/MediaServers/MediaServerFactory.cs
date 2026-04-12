using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using NLog;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.MediaServers
{
    public interface IMediaServerFactory : IProviderFactory<IMediaServer, MediaServerDefinition>
    {
        List<IMediaServer> Enabled(bool filterBlockedProviders = true);
    }

    public class MediaServerFactory : ProviderFactory<IMediaServer, MediaServerDefinition>, IMediaServerFactory
    {
        private readonly IMediaServerStatusService _mediaServerStatusService;
        private readonly Logger _logger;

        public MediaServerFactory(IMediaServerStatusService mediaServerStatusService,
                                   IMediaServerRepository providerRepository,
                                   IEnumerable<IMediaServer> providers,
                                   IServiceProvider container,
                                   IEventAggregator eventAggregator,
                                   Logger logger)
            : base(providerRepository, providers, container, eventAggregator, logger)
        {
            _mediaServerStatusService = mediaServerStatusService;
            _logger = logger;
        }

        protected override List<MediaServerDefinition> Active()
        {
            return base.Active().Where(c => c.Enable).ToList();
        }

        public List<IMediaServer> Enabled(bool filterBlockedProviders = true)
        {
            return GetAvailableProviders().ToList();
        }

        public override ValidationResult Test(MediaServerDefinition definition)
        {
            var result = base.Test(definition);

            if (definition.Id == 0)
            {
                return result;
            }

            if (result == null || result.IsValid)
            {
                _mediaServerStatusService.RecordSuccess(definition.Id);
            }
            else
            {
                _mediaServerStatusService.RecordFailure(definition.Id);
            }

            return result;
        }
    }
}
