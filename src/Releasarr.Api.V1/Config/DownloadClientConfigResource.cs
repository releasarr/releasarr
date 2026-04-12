using NzbDrone.Core.Configuration;
using Releasarr.Http.REST;

namespace Releasarr.Api.V1.Config
{
    public class DownloadClientConfigResource : RestResource
    {
    }

    public static class DownloadClientConfigResourceMapper
    {
        public static DownloadClientConfigResource ToResource(IConfigService model)
        {
            return new DownloadClientConfigResource
            {
            };
        }
    }
}
