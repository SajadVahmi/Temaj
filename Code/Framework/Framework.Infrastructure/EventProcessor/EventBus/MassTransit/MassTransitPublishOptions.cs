namespace Framework.Infrastructure.EventProcessor.EventBus.MassTransit;

public class MassTransitPublishOptions
{
    public string? RabbitMqConnectionString { get; set; }
    public ushort? Port { get; set; }
    public string? VirtualHost { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}
public class MassTransitSendOptions
{
    public string? RabbitMqConnectionString { get; set; }
    public ushort? Port { get; set; }
    public string? VirtualHost { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string?Address { get; set; }
}

