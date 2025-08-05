using Framework.Core.Domain.DomainEvents;

namespace Idp.Domain.UserAggregate.Events;

public record UserRegistered(
    string EventId,
    long UserId,
    string? Email,
    bool? IsEmailConfirmed,
    string? PhoneNumber,
    bool? IsPhoneNumberConfirmed,
    DateTimeOffset TimeOfOccurrence) : DomainEvent(EventId, TimeOfOccurrence);