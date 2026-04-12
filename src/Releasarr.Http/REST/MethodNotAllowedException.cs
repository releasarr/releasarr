using System.Net;
using Releasarr.Http.Exceptions;

namespace Releasarr.Http.REST
{
    public class MethodNotAllowedException : ApiException
    {
        public MethodNotAllowedException(object content = null)
            : base(HttpStatusCode.MethodNotAllowed, content)
        {
        }
    }
}
