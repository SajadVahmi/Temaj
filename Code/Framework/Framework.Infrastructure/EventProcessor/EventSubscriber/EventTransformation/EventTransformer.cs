using Framework.Core.Domain.DomainEvents;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.EventTransformation;

public abstract class EventTransformer<T> : IEventTransformer where T : IEvent
{
    public abstract object Transform(T @event);
    public object TransformEvent(IEvent @event)
    {
        return Transform((T)@event);
    }
}