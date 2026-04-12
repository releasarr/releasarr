using System;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.ArrClients;
using NzbDrone.SignalR;
using Releasarr.Http;

namespace Releasarr.Api.V1.ArrClients
{
    [V1ApiController]
    public class ArrClientController : ProviderControllerBase<ArrClientResource, ArrClientBulkResource, IArrClient, ArrClientDefinition>
    {
        public static readonly ArrClientResourceMapper ResourceMapper = new();
        public static readonly ArrClientBulkResourceMapper BulkResourceMapper = new();

        public ArrClientController(IBroadcastSignalRMessage signalRBroadcaster, ArrClientFactory arrClientFactory)
            : base(signalRBroadcaster, arrClientFactory, "arrclient", ResourceMapper, BulkResourceMapper)
        {
        }

        [NonAction]
        public override ActionResult<ArrClientResource> UpdateProvider([FromBody] ArrClientBulkResource providerResource)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public override object DeleteProviders([FromBody] ArrClientBulkResource resource)
        {
            throw new NotImplementedException();
        }
    }
}
