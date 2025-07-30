namespace Framework.Core.DomainEvents;

public abstract record DomainEvent(string EventId, DateTimeOffset TimeOfOccurrence) : IDomainEvent;
