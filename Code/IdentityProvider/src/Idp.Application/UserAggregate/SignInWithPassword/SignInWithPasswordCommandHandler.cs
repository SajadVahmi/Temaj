using Framework.Core.Application.Commands;
using Framework.Core.Domain.Exceptions;
using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Contracts;
using Idp.Domain.UserAggregate.Arguments;
using Idp.Domain.UserAggregate.Contracts;

namespace Idp.Application.UserAggregate.SignInWithPassword;

public class SignInWithPasswordCommandHandler(
    IUserRepository userRepository,
    IIdGenerator idGenerator,
    IClock clock,
    ISignInService signInService) : ICommandHandler<SignInWithPasswordCommand>
{
    public async Task HandleAsync(SignInWithPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByPhoneNumberAsync(command.PhoneNumber, cancellationToken);

        if (user is null)
            throw new BusinessException("TheUserNameOrPasswordIsNotCorrect");

        await user.SignInWithPasswordAsync(new SignInWithPasswordArgs(
                Password: command.Password,
                SignInService: signInService,
                IdGenerator: idGenerator,
                Clock: clock),
            cancellationToken);

        await userRepository.UpdateAsync(user, cancellationToken);
    }
}
