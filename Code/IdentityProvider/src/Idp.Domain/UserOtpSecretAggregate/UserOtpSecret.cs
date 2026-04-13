using Framework.Core.Domain.Aggregates;
using Framework.Core.Domain.Exceptions;
using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Enums;
using Idp.Domain._Shared.Resources.Exceptions;
using Idp.Domain.UserOtpSecretAggregate.Arguments;
using Idp.Domain.UserOtpSecretAggregate.Contracts;
using Idp.Domain.UserOtpSecretAggregate.Events;

namespace Idp.Domain.UserOtpSecretAggregate;

public class UserOtpSecret : AggregateRoot<long>
{
    public static UserOtpSecret Generate(GenerateUserOtpSecretArgs args) =>
        new(args.UserId, args.Chanel, args.SecretKeyGenerator, args.IdGenerator, args.Clock);

    protected UserOtpSecret() { }

    private UserOtpSecret(long userId, OtpChanel chanel, ISecretKeyGenerator secretKeyGenerator, IIdGenerator idGenerator, IClock clock)
    {
        UserId = userId;
        SecretKey = secretKeyGenerator.GenerateBase32Secret();
        Chanel = chanel;

        Id = idGenerator.GetNewId();

        CheckInvariants();

        AddEvent(new UserOtpSecretGenerated(
            EventId: idGenerator.GetNewId().ToString(),
            UserOtpSecretId: Id,
            UserId: UserId,
            SecretKey: SecretKey,
            Chanel: Chanel,
            TimeOfOccurrence: clock.GetDateTime()));
    }

    public long UserId { get; private set; }
    public string SecretKey { get; private set; } = null!;
    public OtpChanel Chanel { get; private set; }

   

   
    public async Task SendOtpBySmsAsync(SendOtpBySmsArgs args,CancellationToken cancellationToken=default)
    { 
       var userPhoneNumber =await args.UsrPhoneNumberResolver.GetUserPhoneNumberAsync(UserId, cancellationToken);
       
       if (userPhoneNumber == null)
           throw new BusinessException(BusinessExceptions.TheUserPhoneNumberNotFound);
       
       var otpCoe = args.OtpService.GenerateOtp(SecretKey);

       await args.SmsSender.SendSmsAsync(userPhoneNumber, otpCoe, cancellationToken);

       AddEvent(new SmsOtpCodeSentToUser(
           EventId: args.IdGenerator.GetNewId().ToString(),
           UserOtpSecretId: Id,
           UserId: UserId,
           OtpCode: otpCoe,
           Chanel: Chanel,
           TimeOfOccurrence: args.Clock.GetDateTime()));
    }

    protected sealed override void CheckInvariants()
    {

    }

}