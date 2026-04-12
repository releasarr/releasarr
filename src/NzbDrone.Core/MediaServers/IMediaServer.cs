using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.MediaServers
{
    public interface IMediaServer : IProvider
    {
        string Link { get; }
    }
}
