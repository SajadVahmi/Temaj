using Framework.Core.Application.Commands;
using Idp.Domain._Shared.Contracts;
using Idp.Domain._Shared.Enums;

namespace Idp.Application.UserAggregate.SignInWithAuthenticatorCode;

public class SignInWithAuthenticatorCodeCommandHandler(
    ISignInService signInService) : ICommandHandler<SignInWithAuthenticatorCodeCommand, SignInWithAuthenticatorCodeResult>
{
    public async Task<SignInWithAuthenticatorCodeResult> HandleAsync(SignInWithAuthenticatorCodeCommand command, CancellationToken cancellationToken = default)
    {
        var signInStatus = await signInService.SignInWithAuthenticatorCodeAsync(command.VerificationCode, command.RememberMachine, cancellationToken);

        if (signInStatus == TwoFactorSignInStatus.NotAvailable)
            return new SignInWithAuthenticatorCodeResult(false, true);

        if (signInStatus == TwoFactorSignInStatus.Failed)
            return new SignInWithAuthenticatorCodeResult(false, false);

        return new SignInWithAuthenticatorCodeResult(true, false);
    }
}
