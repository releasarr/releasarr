using NzbDrone.Common.Http;

namespace NzbDrone.Common.Cloud
{
    public interface IReleasarrCloudRequestBuilder
    {
        IHttpRequestBuilderFactory Services { get; }
        IHttpRequestBuilderFactory Releases { get; }
    }

    public class ReleasarrCloudRequestBuilder : IReleasarrCloudRequestBuilder
    {
        public ReleasarrCloudRequestBuilder()
        {
            Services = new HttpRequestBuilder("https://releasarr.servarr.com/v1/")
                .CreateFactory();

            Releases = new HttpRequestBuilder("https://releases.servarr.com/v1/")
                .CreateFactory();
        }

        public IHttpRequestBuilderFactory Services { get; private set; }

        public IHttpRequestBuilderFactory Releases { get; private set; }
    }
}
