using Framework.Core.Domain.DomainEvents;

namespace Framework.Infrastructure.EventProcessor.EventBus;

public abstract class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent :class,IEvent
{
    
    public abstract Task HandleEvent(TEvent @event);

}

