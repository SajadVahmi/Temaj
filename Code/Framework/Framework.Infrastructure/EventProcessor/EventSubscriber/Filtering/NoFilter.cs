using Framework.Core.Domain.DomainEvents;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.Filtering;

public class NoFilter : IFilter
{
    public bool ShouldProcess(IEvent @event)
    {
        return true;
    }
}