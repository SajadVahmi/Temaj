using Framework.Core.Application.Commands;

namespace Idp.Application.UserAggregate.ManageAuthenticatorTwoFactor;

public class DisableAuthenticatorTwoFactorCommand : ICommand
{
    public long UserId { get; set; }
}
