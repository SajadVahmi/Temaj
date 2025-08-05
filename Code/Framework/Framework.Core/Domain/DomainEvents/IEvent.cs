namespace Framework.Core.Domain.DomainEvents;

public interface IEvent
{
    string EventId { get; }
    DateTimeOffset TimeOfOccurrence { get; }
}