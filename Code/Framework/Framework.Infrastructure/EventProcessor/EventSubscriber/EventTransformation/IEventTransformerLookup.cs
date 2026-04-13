using Framework.Core.Domain.DomainEvents;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.EventTransformation;

public interface IEventTransformerLookUp
{
    IEventTransformer? LookUpTransformer(IEvent @event);
    Type? LookUpTransformerType(IEvent @event);
}