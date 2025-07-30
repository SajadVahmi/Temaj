namespace Framework.Core.DomainEvents;

public interface IEvent
{
    string EventId { get; }
    DateTimeOffset TimeOfOccurrence { get; }
}