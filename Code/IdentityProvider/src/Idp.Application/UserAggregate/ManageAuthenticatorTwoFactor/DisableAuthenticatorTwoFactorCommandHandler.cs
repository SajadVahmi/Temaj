using Framework.Core.Application.Commands;
using Framework.Core.Domain.Exceptions;
using Idp.Domain._Shared.Contracts;
using Idp.Domain.UserAggregate.Contracts;

namespace Idp.Application.UserAggregate.ManageAuthenticatorTwoFactor;

public class DisableAuthenticatorTwoFactorCommandHandler(
    IUserRepository userRepository,
    ISignInService signInService) : ICommandHandler<DisableAuthenticatorTwoFactorCommand>
{
    public async Task HandleAsync(DisableAuthenticatorTwoFactorCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException();

        await signInService.DisableAuthenticatorTwoFactorAsync(user, cancellationToken);
    }
}
