using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.ArrClients
{
    public interface IArrClient : IProvider
    {
        string Link { get; }
    }
}
