using Framework.Core.Application.Commands;

namespace Idp.Application.UserAggregate.ManageAuthenticatorTwoFactor;

public class GetAuthenticatorSetupCommand : ICommand<GetAuthenticatorSetupResult>
{
    public long UserId { get; set; }
    public string Issuer { get; set; } = null!;
}
