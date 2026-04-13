namespace Framework.Infrastructure.EventProcessor.EventBus;

public interface IEventHandler<in TEvent> where TEvent : class
{
     Task HandleEvent(TEvent @event);
}

public interface IRequestHandler<in TRequest> where TRequest : class
{
    Task ConsumeMessage(TRequest message);
}