namespace Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore.SqlServer;

public class SqlServerEventStoreSubscriberConfig
{
    public string? EventTable { get; set; }
    public string? CursorTable { get; set; }
    public int? PullingInterval { get; set; }
    public string? ConnectionString { get; set; }
}