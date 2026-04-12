using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.ArrClients
{
    public class ArrClientDefinition : ProviderDefinition
    {
        public override bool Enable { get; set; }
    }
}
