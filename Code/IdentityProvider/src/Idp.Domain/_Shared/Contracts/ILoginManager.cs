using Framework.Core.Domain.Services;
using Idp.Domain.UserAggregate;

namespace Idp.Domain._Shared.Contracts;

public interface ISignInService:IDomainService
{
    public Task<bool> ValidatePasswordAsync(User user, string password, CancellationToken cancellationToken = default);
    public Task SignInAsync(User user, CancellationToken cancellationToken = default);
    public Task SignOutAsync(CancellationToken cancellationToken = default);
}
