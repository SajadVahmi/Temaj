using Framework.Core.Domain.DomainEvents;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.Filtering;

public interface IFilter
{
    bool ShouldProcess(IEvent @event);
}