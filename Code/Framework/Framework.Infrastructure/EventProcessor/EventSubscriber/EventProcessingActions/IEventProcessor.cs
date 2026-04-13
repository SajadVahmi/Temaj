namespace Framework.Infrastructure.EventProcessor.EventSubscriber.EventProcessingActions;

public interface  IEventProcessor
{
    public Task ProcessAsync<T>(T @event, CancellationToken cancellationToken = default) where T : notnull;
    public Task StartAsync(CancellationToken cancellationToken=default);
}