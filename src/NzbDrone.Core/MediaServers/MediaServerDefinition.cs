using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.MediaServers
{
    public class MediaServerDefinition : ProviderDefinition
    {
        public override bool Enable { get; set; }
    }
}
