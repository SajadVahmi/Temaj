using Framework.Core.Application.Commands;
using Framework.Core.Domain.Exceptions;
using Idp.Domain._Shared.Contracts;
using Idp.Domain.UserAggregate.Contracts;

namespace Idp.Application.UserAggregate.ManageAuthenticatorTwoFactor;

public class GetAuthenticatorSetupCommandHandler(
    IUserRepository userRepository,
    ISignInService signInService) : ICommandHandler<GetAuthenticatorSetupCommand, GetAuthenticatorSetupResult>
{
    public async Task<GetAuthenticatorSetupResult> HandleAsync(GetAuthenticatorSetupCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException();

        var setupInfo = await signInService.GetOrCreateAuthenticatorSetupInfoAsync(user, command.Issuer, cancellationToken);

        if (setupInfo is null)
            throw new NotFoundException();

        return new GetAuthenticatorSetupResult(
            SharedKey: setupInfo.SharedKey,
            QrCodeUri: setupInfo.QrCodeUri,
            AccountName: setupInfo.AccountName,
            IsTwoFactorEnabled: setupInfo.IsTwoFactorEnabled);
    }
}
