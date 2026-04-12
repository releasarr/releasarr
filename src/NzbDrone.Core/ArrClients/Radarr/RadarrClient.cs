using System;
using System.Collections.Generic;
using FluentValidation.Results;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.ArrClients.Radarr
{
    public class RadarrClient : IArrClient
    {
        private readonly IRadarrProxy _proxy;

        public RadarrClient(IRadarrProxy proxy)
        {
            _proxy = proxy;
        }

        public string Name => "Radarr";
        public string Link => "https://radarr.video/";

        public Type ConfigContract => typeof(RadarrSettings);

        public ProviderMessage Message => null;

        public IEnumerable<ProviderDefinition> DefaultDefinitions => new List<ProviderDefinition>();

        public ProviderDefinition Definition { get; set; }

        public RadarrSettings Settings => (RadarrSettings)Definition.Settings;

        public ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();
            failures.AddIfNotNull(_proxy.Test(Settings));
            return new ValidationResult(failures);
        }

        public object RequestAction(string stage, IDictionary<string, string> query)
        {
            return null;
        }
    }
}
