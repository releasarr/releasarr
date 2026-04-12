using System;
using System.Collections.Generic;
using FluentValidation.Results;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.ArrClients.Sonarr
{
    public class SonarrClient : IArrClient
    {
        private readonly ISonarrProxy _proxy;

        public SonarrClient(ISonarrProxy proxy)
        {
            _proxy = proxy;
        }

        public string Name => "Sonarr";
        public string Link => "https://sonarr.tv/";

        public Type ConfigContract => typeof(SonarrSettings);

        public ProviderMessage Message => null;

        public IEnumerable<ProviderDefinition> DefaultDefinitions => new List<ProviderDefinition>();

        public ProviderDefinition Definition { get; set; }

        public SonarrSettings Settings => (SonarrSettings)Definition.Settings;

        public ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();
            failures.AddIfNotNull(_proxy.Test(Settings));
            return new ValidationResult(failures);
        }

        public object RequestAction(string stage, IDictionary<string, string> query)
        {
            if (stage == "getTags")
            {
                var tags = _proxy.GetTags(Settings);
                return new
                {
                    options = tags.ConvertAll(t => new
                    {
                        value = t.Id,
                        name = t.Label
                    })
                };
            }

            return null;
        }
    }
}
