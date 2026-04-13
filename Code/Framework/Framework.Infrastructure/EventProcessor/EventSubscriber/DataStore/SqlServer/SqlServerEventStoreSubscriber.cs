using System.Timers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore.SqlServer;

public class SqlServerEventStoreSubscriber : IEventStoreSubscriber
{
    private IEventStoreChangeTracker _changeTracker = null!;
    private readonly string _subscriptionName;
    private readonly ILogger<SqlServerEventStoreSubscriber> _logger;
    private readonly SqlServerEventStoreSubscriberConfig _subscriberConfig;
    private readonly Timer _timer;
    public SqlServerEventStoreSubscriber(string subscriptionName,SqlServerEventStoreSubscriberConfig sqlStoreConfig, ILogger<SqlServerEventStoreSubscriber> logger)
    {
        _subscriptionName = subscriptionName;
        _logger = logger;
        _subscriberConfig = sqlStoreConfig;
        _timer = new Timer(_subscriberConfig.PullingInterval!.Value);
        _timer.Elapsed += TimerOnElapsed!;
    }
    public void SetSubscriber(IEventStoreChangeTracker changeTracker)
    {
        this._changeTracker = changeTracker;
    }

    public ISubscription SubscribeForChanges()
    {
        _timer.Start();
        return new ActionSubscription(_timer.Stop);
    }

    private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        _timer.Stop();

        await using var connection = new SqlConnection(_subscriberConfig.ConnectionString);
        {
            var cursorPosition = connection.GetCursorPosition(_subscriberConfig, _subscriptionName);

            var events = connection.GetEventsFromPosition(_subscriberConfig, cursorPosition);

            if (events.Any())
            {
                _logger.LogInformation($"{events.Count} Events found in Tables for {_subscriptionName}");

                await _changeTracker.ChangeDetected(events);

                connection.MovePosition(_subscriberConfig, events.Last().Id, _subscriptionName);

                _logger.LogInformation($"Cursor moved to position {events.Last().Id} for {_subscriptionName}");
            }
        }
        
        _timer.Start();
    }
}