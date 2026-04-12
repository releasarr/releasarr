using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using NLog;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.ArrClients
{
    public interface IArrClientFactory : IProviderFactory<IArrClient, ArrClientDefinition>
    {
        List<IArrClient> Enabled(bool filterBlockedProviders = true);
    }

    public class ArrClientFactory : ProviderFactory<IArrClient, ArrClientDefinition>, IArrClientFactory
    {
        private readonly IArrClientStatusService _arrClientStatusService;
        private readonly Logger _logger;

        public ArrClientFactory(IArrClientStatusService arrClientStatusService,
                                 IArrClientRepository providerRepository,
                                 IEnumerable<IArrClient> providers,
                                 IServiceProvider container,
                                 IEventAggregator eventAggregator,
                                 Logger logger)
            : base(providerRepository, providers, container, eventAggregator, logger)
        {
            _arrClientStatusService = arrClientStatusService;
            _logger = logger;
        }

        protected override List<ArrClientDefinition> Active()
        {
            return base.Active().Where(c => c.Enable).ToList();
        }

        public List<IArrClient> Enabled(bool filterBlockedProviders = true)
        {
            return GetAvailableProviders().ToList();
        }

        public override ValidationResult Test(ArrClientDefinition definition)
        {
            var result = base.Test(definition);

            if (definition.Id == 0)
            {
                return result;
            }

            if (result == null || result.IsValid)
            {
                _arrClientStatusService.RecordSuccess(definition.Id);
            }
            else
            {
                _arrClientStatusService.RecordFailure(definition.Id);
            }

            return result;
        }
    }
}
