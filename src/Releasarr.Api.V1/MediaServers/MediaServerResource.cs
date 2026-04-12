using NzbDrone.Core.MediaServers;

namespace Releasarr.Api.V1.MediaServers
{
    public class MediaServerResource : ProviderResource<MediaServerResource>
    {
    }

    public class MediaServerResourceMapper : ProviderResourceMapper<MediaServerResource, MediaServerDefinition>
    {
        public override MediaServerResource ToResource(MediaServerDefinition definition)
        {
            if (definition == null)
            {
                return default(MediaServerResource);
            }

            var resource = base.ToResource(definition);

            return resource;
        }

        public override MediaServerDefinition ToModel(MediaServerResource resource, MediaServerDefinition existingDefinition)
        {
            if (resource == null)
            {
                return default(MediaServerDefinition);
            }

            var definition = base.ToModel(resource, existingDefinition);

            return definition;
        }
    }
}
