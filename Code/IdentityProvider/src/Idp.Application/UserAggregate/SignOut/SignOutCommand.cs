using Framework.Core.Application.Commands;
using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Contracts;
using Idp.Domain.UserAggregate.Arguments;
using Idp.Domain.UserAggregate.Contracts;

namespace Idp.Application.UserAggregate.SignOut;

public class SignOutCommand:ICommand;

public class SignOutCommandHandler(
    IUserRepository userRepository,
    IIdGenerator idGenerator,
    IClock clock,
    IIdentityService identityService,
    ISignInService signInService) : ICommandHandler<SignOutCommand>
{
    public async Task HandleAsync(SignOutCommand command, CancellationToken cancellationToken = default)
    {
        var userId= identityService.CurrentUserId;
        if(userId is null)
            return;

        var user = await userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
            return;

        await user.SignOutAsync(new SignOutArgs(
            SignInService: signInService,
            IdGenerator: idGenerator,
            Clock: clock),
            cancellationToken);

        await userRepository.UpdateAsync(user, cancellationToken);

    }
}