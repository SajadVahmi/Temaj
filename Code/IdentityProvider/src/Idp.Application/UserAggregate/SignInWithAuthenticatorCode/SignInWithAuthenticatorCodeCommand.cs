using Framework.Core.Application.Commands;

namespace Idp.Application.UserAggregate.SignInWithAuthenticatorCode;

public class SignInWithAuthenticatorCodeCommand : ICommand<SignInWithAuthenticatorCodeResult>
{
    public string VerificationCode { get; set; } = null!;
    public bool RememberMachine { get; set; }
}
