using NzbDrone.Core.MediaServers;

namespace Releasarr.Api.V1.MediaServers
{
    public class MediaServerBulkResource : ProviderBulkResource<MediaServerBulkResource>
    {
    }

    public class MediaServerBulkResourceMapper : ProviderBulkResourceMapper<MediaServerBulkResource, MediaServerDefinition>
    {
    }
}
