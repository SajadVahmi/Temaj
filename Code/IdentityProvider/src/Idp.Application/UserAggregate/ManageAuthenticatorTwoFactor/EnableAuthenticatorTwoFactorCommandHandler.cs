using Framework.Core.Application.Commands;
using Framework.Core.Domain.Exceptions;
using Idp.Domain._Shared.Contracts;
using Idp.Domain.UserAggregate.Contracts;

namespace Idp.Application.UserAggregate.ManageAuthenticatorTwoFactor;

public class EnableAuthenticatorTwoFactorCommandHandler(
    IUserRepository userRepository,
    ISignInService signInService) : ICommandHandler<EnableAuthenticatorTwoFactorCommand, EnableAuthenticatorTwoFactorResult>
{
    public async Task<EnableAuthenticatorTwoFactorResult> HandleAsync(EnableAuthenticatorTwoFactorCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException();

        var enabled = await signInService.EnableAuthenticatorTwoFactorAsync(user, command.VerificationCode, cancellationToken);

        return new EnableAuthenticatorTwoFactorResult(enabled);
    }
}
