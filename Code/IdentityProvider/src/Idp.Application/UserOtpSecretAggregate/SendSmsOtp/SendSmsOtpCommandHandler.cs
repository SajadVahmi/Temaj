using Framework.Core.Application.Commands;
using Framework.Core.Domain.Repositories;
using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Contracts;
using Idp.Domain._Shared.Enums;
using Idp.Domain.UserAggregate.Contracts;
using Idp.Domain.UserOtpSecretAggregate;
using Idp.Domain.UserOtpSecretAggregate.Arguments;
using Idp.Domain.UserOtpSecretAggregate.Contracts;

namespace Idp.Application.UserOtpSecretAggregate.SendSmsOtp;

public class SendSmsOtpCommandHandler(
    IUserOtpSecretRepository repository,
    IIdGenerator idGenerator,
    ISecretKeyGenerator secretKeyGenerator,
    ISmsSender smsSender,
    IUserPhoneNumberResolver phoneNumberResolver,
    IOtpService otpService,
    IUnitOfWork unitOfWork,
    IClock clock
) : ICommandHandler<SendSmsOtpCommand>
{
    public async Task HandleAsync(SendSmsOtpCommand command, CancellationToken cancellationToken = default)
    {
        var userOtpSecret = await repository.GetByChanelAsync(command.UserId, OtpChanel.Sms, cancellationToken);

        if (userOtpSecret is null)
        {
            userOtpSecret = UserOtpSecret.Generate(new GenerateUserOtpSecretArgs(
                UserId: command.UserId,
                Chanel: OtpChanel.Sms,
                SecretKeyGenerator: secretKeyGenerator,
                IdGenerator: idGenerator,
                Clock: clock));

            repository.Add(userOtpSecret);
        }

        await userOtpSecret.SendOtpBySmsAsync(new SendOtpBySmsArgs(
            SmsSender: smsSender,
            OtpService: otpService,
            UsrPhoneNumberResolver: phoneNumberResolver,
            IdGenerator: idGenerator,
            Clock: clock), cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}