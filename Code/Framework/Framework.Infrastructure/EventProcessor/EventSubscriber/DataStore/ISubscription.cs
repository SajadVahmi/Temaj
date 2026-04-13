namespace Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore
{
    public interface ISubscription : IDisposable
    {
        void CancelSubscription();
    }
}