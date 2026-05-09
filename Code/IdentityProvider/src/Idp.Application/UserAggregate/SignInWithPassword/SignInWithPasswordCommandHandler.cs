using Framework.Core.Application.Commands;
using Framework.Core.Domain.Exceptions;
using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Contracts;
using Idp.Domain._Shared.Enums;
using Idp.Domain.UserAggregate.Arguments;
using Idp.Domain.UserAggregate.Contracts;

namespace Idp.Application.UserAggregate.SignInWithPassword;

public class SignInWithPasswordCommandHandler(
    IUserRepository userRepository,
    IIdGenerator idGenerator,
    IClock clock,
    ISignInService signInService) : ICommandHandler<SignInWithPasswordCommand, SignInWithPasswordResult>
{
    public async Task<SignInWithPasswordResult> HandleAsync(SignInWithPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByPhoneNumberOrEmailAsync(command.PhoneNumberOrEmail, cancellationToken);

        if (user is null)
            throw new BusinessException("TheUserNameOrPasswordIsNotCorrect");

        var signInStatus = await user.SignInWithPasswordAsync(new SignInWithPasswordArgs(
                Password: command.Password,
                SignInService: signInService,
                IdGenerator: idGenerator,
                Clock: clock),
            cancellationToken);

        if (signInStatus == PasswordSignInStatus.Succeeded)
            await userRepository.UpdateAsync(user, cancellationToken);

        return new SignInWithPasswordResult(RequiresTwoFactor: signInStatus == PasswordSignInStatus.RequiresTwoFactor);
    }
}
