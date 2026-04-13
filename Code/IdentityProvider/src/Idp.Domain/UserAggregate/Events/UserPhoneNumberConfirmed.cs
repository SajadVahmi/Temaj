using Framework.Core.Domain.DomainEvents;

namespace Idp.Domain.UserAggregate.Events;

public record UserPhoneNumberConfirmed(
    string EventId,
    long UserId,
    string? PhoneNumber,
    bool? IsPhoneNumberConfirmed,
    DateTimeOffset TimeOfOccurrence) : DomainEvent(EventId, TimeOfOccurrence);