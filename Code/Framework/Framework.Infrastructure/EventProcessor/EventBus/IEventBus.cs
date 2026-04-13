namespace Framework.Infrastructure.EventProcessor.EventBus;

public interface IEventBus
{
    Task Publish<T>(T @event) where T : notnull;
    Task Start();
}