using MassTransit;

namespace Framework.Infrastructure.EventProcessor.EventBus.MassTransit;

public class MassTransitBusAdapter : IEventBus
{
    private readonly MassTransitPublishOptions _publishOptions;
    private bool _isStarted;
    private IBusControl _bus = null!;

    public MassTransitBusAdapter(MassTransitPublishOptions publishOptions)
    {
        ArgumentNullException.ThrowIfNull(publishOptions);
        ArgumentNullException.ThrowIfNull(publishOptions.Port);
        _publishOptions = publishOptions;
        _isStarted = false;
    }
    public Task Publish<T>(T @event) where T : notnull
    {
        if (!_isStarted) throw new BusNotStartedException();
        return _bus.Publish(@event);
    }
    public async Task Start()
    {
        _bus =  Bus.Factory.CreateUsingRabbitMq(sbc =>
        {
            sbc.Host(host:_publishOptions.RabbitMqConnectionString,port:_publishOptions.Port!.Value,virtualHost:_publishOptions.VirtualHost,connectionName:null, h =>
            {
                h.Username(_publishOptions.UserName!);
                h.Password(_publishOptions.Password!);
            });
        });
        await _bus.StartAsync();
        this._isStarted = true;
    }
}