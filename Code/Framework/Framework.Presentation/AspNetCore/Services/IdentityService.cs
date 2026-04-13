using System.Security.Claims;
using Framework.Core.Domain.Exceptions;
using Framework.Core.Domain.Services;
using Microsoft.AspNetCore.Http;

namespace Framework.Presentation.AspNetCore.Services
{
    public class IdentityService(IHttpContextAccessor httpContextAccessor) : IIdentityService
    {
        private HttpContext? HttpContext => httpContextAccessor.HttpContext;

        public long? CurrentUserId
        {
            get
            {
                if (Claims != null)
                    return !IsAuthenticated ? null : long.Parse(Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
                return null;
            }
        }

        
       
        public bool IsAuthenticated => HttpContext?.User.Identity?.IsAuthenticated?? false;

        public long RequiredCurrentUserId => CurrentUserId!.Value;

        public Guid RequiredDeviceId => Guid.Parse(Claims!.Single(x => x.Type == "DeviceId").Value);

        public List<Claim>? Claims => HttpContext?.User.Claims.ToList();

        public void CheckAccess(string scopeType, object scopeId)
        {
            if (Claims != null && Claims.Any(x => x.Type == "FullScope" && x.Value == true.ToString()))
                return;

            if (Claims != null && Claims.Any(x => x.Type.ToLower() == scopeType.ToLower() && x.Value.ToLower() == scopeId.ToString()?.ToLower()))
                return;

            throw new ForbiddenException();
        }
    }
}
