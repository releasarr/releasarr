using System;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.MediaServers;
using NzbDrone.SignalR;
using Releasarr.Http;

namespace Releasarr.Api.V1.MediaServers
{
    [V1ApiController]
    public class MediaServerController : ProviderControllerBase<MediaServerResource, MediaServerBulkResource, IMediaServer, MediaServerDefinition>
    {
        public static readonly MediaServerResourceMapper ResourceMapper = new();
        public static readonly MediaServerBulkResourceMapper BulkResourceMapper = new();

        public MediaServerController(IBroadcastSignalRMessage signalRBroadcaster, MediaServerFactory mediaServerFactory)
            : base(signalRBroadcaster, mediaServerFactory, "mediaserver", ResourceMapper, BulkResourceMapper)
        {
        }

        [NonAction]
        public override ActionResult<MediaServerResource> UpdateProvider([FromBody] MediaServerBulkResource providerResource)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public override object DeleteProviders([FromBody] MediaServerBulkResource resource)
        {
            throw new NotImplementedException();
        }
    }
}
