using NzbDrone.Core.ArrClients;

namespace Releasarr.Api.V1.ArrClients
{
    public class ArrClientResource : ProviderResource<ArrClientResource>
    {
    }

    public class ArrClientResourceMapper : ProviderResourceMapper<ArrClientResource, ArrClientDefinition>
    {
    }
}
