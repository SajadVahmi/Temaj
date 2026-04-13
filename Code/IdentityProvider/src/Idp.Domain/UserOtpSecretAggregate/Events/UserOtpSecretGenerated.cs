using Framework.Core.Domain.DomainEvents;
using Idp.Domain._Shared.Enums;

namespace Idp.Domain.UserOtpSecretAggregate.Events;

public record UserOtpSecretGenerated(
    string EventId,
    long UserOtpSecretId,
    long UserId,
    string SecretKey,
    OtpChanel Chanel,
    DateTimeOffset TimeOfOccurrence) : DomainEvent(EventId, TimeOfOccurrence);


public record SmsOtpCodeSentToUser(
    string EventId,
    long UserOtpSecretId,
    long UserId,
    string OtpCode,
    OtpChanel Chanel,
    DateTimeOffset TimeOfOccurrence) : DomainEvent(EventId, TimeOfOccurrence);