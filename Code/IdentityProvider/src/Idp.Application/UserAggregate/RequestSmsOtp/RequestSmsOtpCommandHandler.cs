using Framework.Core.Application.Commands;
using Framework.Core.Domain.Repositories;
using Framework.Core.Domain.Services;
using Idp.Domain.UserAggregate;
using Idp.Domain.UserAggregate.Arguments;
using Idp.Domain.UserAggregate.Contracts;

namespace Idp.Application.UserAggregate.RequestSmsOtp;

public class RequestSmsOtpCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IIdGenerator idGenerator,
    IClock clock
) : ICommandHandler<RequestSmsOtpCommand>
{
    public async Task HandleAsync(RequestSmsOtpCommand command, CancellationToken cancellationToken = default)
    {
        var user =
            await userRepository.GetByPhoneNumberAsync(command.PhoneNumber, cancellationToken) ?? User.Register(
                new RegisterUserArgs(
                    PhoneNumber: command.PhoneNumber,
                    IdGenerator: idGenerator,
                    Clock: clock));

        user.RequestSmsOtp(new RequestSmsOtpArgs(
            IdGenerator: idGenerator,
            Clock: clock));

        await userRepository.UpdateAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}