using System.Security.Claims;

namespace Framework.Core.Domain.Services;

public interface IIdentityService
{
    public long? CurrentUserId { get; }
    public long? CurrentAccountId { get; }
    public long RequiredCurrentUserId { get; }
    public Guid RequiredDeviceId { get; }
    public bool IsAuthenticated { get; }
    public List<Claim>? Claims { get; }
    void CheckAccess(string scopeType, object scopeId);
}