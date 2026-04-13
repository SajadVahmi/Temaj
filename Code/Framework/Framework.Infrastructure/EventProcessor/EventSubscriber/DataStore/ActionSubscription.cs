namespace Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore;

public class ActionSubscription(Action cancel) : ISubscription
{
    public void CancelSubscription()
    {
        cancel.Invoke();
    }
    public void Dispose()
    {
        CancelSubscription();
    }
}