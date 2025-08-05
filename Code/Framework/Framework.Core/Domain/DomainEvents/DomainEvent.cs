namespace Framework.Core.Domain.DomainEvents;

public abstract record DomainEvent(string EventId, DateTimeOffset TimeOfOccurrence) : IDomainEvent;
