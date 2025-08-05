using Framework.Core.Domain.Aggregates;
using Framework.Core.Domain.DomainEvents;
using Framework.Core.Domain.Services;

namespace Framework.Infrastructure.Persistence.Events;

public static class EventItemFactory
{
    public static List<EventItem> Create(IAggregateRoot aggregateRoot, IEnumerable<IDomainEvent> domainEvents, IJsonSerializerAdapter serializer, IIdentityService identityService)
    {
        return domainEvents.Select(domainEvent => Create(aggregateRoot, domainEvent, serializer, identityService)).ToList();
    }

    private static EventItem Create(IAggregateRoot aggregateRoot, IDomainEvent domainEvent, IJsonSerializerAdapter serializer, IIdentityService identityService)
    {
        return new EventItem()
        {
            EventId = domainEvent.EventId,
            OccurredByUserId = identityService.CurrentUserId?.ToString(),
            TimeOfOccurrence = domainEvent.TimeOfOccurrence,
            AggregateName = aggregateRoot.GetType().Name,
            AggregateTypeName = aggregateRoot.GetType().FullName!,
            EventName = domainEvent.GetType().Name,
            EventTypeName = domainEvent.GetType().FullName!,
            EventPayload = serializer.Serialize(domainEvent)!
        };
    }
}
