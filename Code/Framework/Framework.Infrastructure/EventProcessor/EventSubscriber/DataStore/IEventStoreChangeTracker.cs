using Framework.Infrastructure.Persistence.Events;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore;

public interface IEventStoreChangeTracker
{
    Task ChangeDetected(List<EventItem> item);
}