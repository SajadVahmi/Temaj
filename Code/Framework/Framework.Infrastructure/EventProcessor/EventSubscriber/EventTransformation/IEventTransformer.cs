using Framework.Core.Domain.DomainEvents;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.EventTransformation;

public interface IEventTransformer
{
    object TransformEvent(IEvent @event);

}