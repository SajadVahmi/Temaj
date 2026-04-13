using Framework.Core.Application.Commands;
using Framework.Core.Domain.Exceptions;
using Framework.Core.Domain.Repositories;
using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Contracts;
using Idp.Domain.UserAggregate.Arguments;
using Idp.Domain.UserAggregate.Contracts;
using Idp.Domain.UserOtpSecretAggregate.Contracts;

namespace Idp.Application.UserAggregate.SignInWithSmsOtp;

public class SignInWithSmsOtpCommandHandler(
    IUserRepository userRepository,
    IUserOtpSecretRepository otpSecretRepository,
    IIdGenerator idGenerator,
    IOtpService otpService,
    IClock clock,
    ISignInService signInService) : ICommandHandler<SignInWithSmsOtpCommand>
{
    public async Task HandleAsync(SignInWithSmsOtpCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByPhoneNumberAsync(command.PhoneNumber, cancellationToken);

        if (user is null)
            throw new BusinessException("Invalid phone number");

        await user.SignInWithSmsOtpAsync(new SignInWithSmsOtpArgs(
                OtpCode: command.OtpCode,
                OtpSecretRepository: otpSecretRepository,
                OtpService: otpService,
                SignInService: signInService,
                IdGenerator: idGenerator,
                Clock: clock),
            cancellationToken
        );

        await userRepository.UpdateAsync(user, cancellationToken);


    }
}