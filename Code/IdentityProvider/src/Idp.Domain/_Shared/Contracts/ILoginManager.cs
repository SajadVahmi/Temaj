using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Enums;
using Idp.Domain.UserAggregate;

namespace Idp.Domain._Shared.Contracts;

public interface ISignInService : IDomainService
{
    Task<bool> ValidatePasswordAsync(User user, string password, CancellationToken cancellationToken = default);
    Task<PasswordSignInStatus> SignInWithPasswordAsync(User user, string password, CancellationToken cancellationToken = default);
    Task<TwoFactorSignInStatus> SignInWithAuthenticatorCodeAsync(string verificationCode, bool rememberMachine, CancellationToken cancellationToken = default);
    Task<bool> HasPendingTwoFactorSignInAsync(CancellationToken cancellationToken = default);
    Task<AuthenticatorSetupInfo?> GetOrCreateAuthenticatorSetupInfoAsync(User user, string issuer, CancellationToken cancellationToken = default);
    Task<bool> EnableAuthenticatorTwoFactorAsync(User user, string verificationCode, CancellationToken cancellationToken = default);
    Task DisableAuthenticatorTwoFactorAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> IsAuthenticatorTwoFactorEnabledAsync(User user, CancellationToken cancellationToken = default);
    Task SignInAsync(User user, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
}
