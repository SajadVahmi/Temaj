using Framework.Core.Domain.Aggregates;
using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Enums;
using Idp.Domain.UserOtpSecretAggregate.Arguments;
using Idp.Domain.UserOtpSecretAggregate.Contracts;
using Idp.Domain.UserOtpSecretAggregate.Events;

namespace Idp.Domain.UserOtpSecretAggregate;

public class UserOtpSecret : AggregateRoot<long>
{
    public static UserOtpSecret Define(DefineUserOtpSecretArgs args) =>
        new(args.UserId, args.Chanel, args.SecretKeyGenerator, args.IdGenerator, args.Clock);

    protected UserOtpSecret() { }

    private UserOtpSecret(long userId, OtpChanel chanel, ISecretKeyGenerator secretKeyGenerator, IIdGenerator idGenerator, IClock clock)
    {
        UserId = userId;
        SecretKey = secretKeyGenerator.GenerateBase32Secret();
        Chanel = chanel;

        Id = idGenerator.GetNewId();

        CheckInvariants();

        AddEvent(new UserOtpSecretDefined(
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

   

    protected sealed override void CheckInvariants()
    {

    }
}