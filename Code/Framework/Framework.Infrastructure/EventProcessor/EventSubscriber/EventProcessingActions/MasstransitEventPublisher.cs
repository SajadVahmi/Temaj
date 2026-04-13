using Framework.Infrastructure.EventProcessor.EventBus;
using Framework.Infrastructure.EventProcessor.EventBus.MassTransit;
using MassTransit;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.EventProcessingActions;

public class MasstransitEventPublisher: IEventProcessor
{
    private readonly MassTransitPublishOptions _publishOptions;
    private bool _isStarted;
    private IBusControl _bus = null!;
    public MasstransitEventPublisher(MassTransitPublishOptions publishOptions)
    {
        ArgumentNullException.ThrowIfNull(publishOptions);
        ArgumentNullException.ThrowIfNull(publishOptions.Port);
        _publishOptions = publishOptions;
        _isStarted = false;
    }
    public async Task ProcessAsync<T>(T @event,CancellationToken cancellationToken=default) where T : notnull
    {
        if (!_isStarted) throw new BusNotStartedException();
        await _bus.Publish(@event,cancellationToken);
    }
    public async Task StartAsync(CancellationToken cancellationToken=default)
    {
        _bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
        {
            sbc.Host(host: _publishOptions.RabbitMqConnectionString, port: _publishOptions.Port!.Value, virtualHost: _publishOptions.VirtualHost, connectionName: null, h =>
            {
                h.Username(_publishOptions.UserName!);
                h.Password(_publishOptions.Password!);
            });
        });
        await _bus.StartAsync(cancellationToken);
        this._isStarted = true;
       
    }
}

public class MasstransitEventSender : IEventProcessor
{
    private readonly MassTransitSendOptions _sendOptions;
    private bool _isStarted;
    private IBusControl _bus = null!;
    public MasstransitEventSender(MassTransitSendOptions sendOptions)
    {
        ArgumentNullException.ThrowIfNull(sendOptions);
        ArgumentNullException.ThrowIfNull(sendOptions.Port);
        ArgumentNullException.ThrowIfNull(sendOptions.Address);
        _sendOptions = sendOptions;
        _isStarted = false;
    }
    public async Task ProcessAsync<T>(T @event, CancellationToken cancellationToken = default) where T : notnull
    {
        if (!_isStarted) throw new BusNotStartedException();
        var endpoint =await _bus.GetSendEndpoint(new Uri(_sendOptions.Address!));//"queue:input-queue"
        await endpoint.Send(@event,cancellationToken);
    }
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
        {
            sbc.Host(host: _sendOptions.RabbitMqConnectionString, port: _sendOptions.Port!.Value, virtualHost: _sendOptions.VirtualHost, connectionName: null, h =>
            {
                h.Username(_sendOptions.UserName!);
                h.Password(_sendOptions.Password!);
            });
        });
        
        await _bus.StartAsync(cancellationToken);
        this._isStarted = true;

    }
}