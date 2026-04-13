namespace Framework.Infrastructure.EventProcessor.Idempotence;

public interface IDuplicateEventHandler
{
    Task<bool> HasMessageBeenProcessed(string consumerName, string messageId);
    Task MarkMessageAsProcessed(string messageId, string consumerName, string typeName, string payload, DateTimeOffset receivedDateTime);
}