using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Releasarr.Http.Authentication
{
    public class BypassableDenyAnonymousAuthorizationRequirement : DenyAnonymousAuthorizationRequirement
    {
    }
}
