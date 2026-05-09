using Framework.Core.Application.Commands;

namespace Idp.Application.UserAggregate.ManageAuthenticatorTwoFactor;

public class EnableAuthenticatorTwoFactorCommand : ICommand<EnableAuthenticatorTwoFactorResult>
{
    public long UserId { get; set; }
    public string VerificationCode { get; set; } = null!;
}
