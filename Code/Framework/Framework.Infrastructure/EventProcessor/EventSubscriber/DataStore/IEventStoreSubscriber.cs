namespace Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore;

public interface IEventStoreSubscriber
{
    void SetSubscriber(IEventStoreChangeTracker changeTracker);
    ISubscription SubscribeForChanges();
}