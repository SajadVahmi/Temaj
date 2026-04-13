using Framework.Core.Domain.DomainEvents;

namespace Idp.Domain.UserAggregate.Events;

public record SmsOtpRequested(
    string EventId,
    long UserId,
    DateTimeOffset TimeOfOccurrence
    ) :DomainEvent(EventId,TimeOfOccurrence);