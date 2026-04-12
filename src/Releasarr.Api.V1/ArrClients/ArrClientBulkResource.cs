using NzbDrone.Core.ArrClients;

namespace Releasarr.Api.V1.ArrClients
{
    public class ArrClientBulkResource : ProviderBulkResource<ArrClientBulkResource>
    {
    }

    public class ArrClientBulkResourceMapper : ProviderBulkResourceMapper<ArrClientBulkResource, ArrClientDefinition>
    {
    }
}
