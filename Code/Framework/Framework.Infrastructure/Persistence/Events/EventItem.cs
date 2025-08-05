namespace Framework.Infrastructure.Persistence.Events;

public class EventItem
{
    public long Id { get; set; }
    public string EventId { get; set; } = default!;
    public string? OccurredByUserId { get; set; }
    public DateTimeOffset TimeOfOccurrence { get; set; }
    public string AggregateName { get; set; } = default!;
    public string AggregateTypeName { get; set; } = default!;
    public string EventName { get; set; } = default!;
    public string EventTypeName { get; set; } = default!;
    public string EventPayload { get; set; } = default!;
}