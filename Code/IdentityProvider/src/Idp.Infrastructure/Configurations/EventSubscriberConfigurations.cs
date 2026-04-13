using Framework.Infrastructure.EventProcessor.EventSubscriber.Setup;
using Framework.Infrastructure.EventProcessor.Idempotence.SqlServer;
using Idp.Domain.UserAggregate.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Idp.Infrastructure.Configurations;

public static class EventSubscriberConfigurations
{
    public static IServiceCollection AddEventPublisher(this IServiceCollection services, IConfiguration configuration)
    {

        var subscriptionName = configuration.GetSection("EventSubscriber:SubscriptionName").Get<string?>();
        var eventDatabase = configuration.GetSection("Infrastructures:SqlServer").Get<string?>();
        var eventTable = configuration.GetSection("EventSubscriber:EventTable").Get<string?>();
        var cursorTable = configuration.GetSection("EventSubscriber:CursorTable").Get<string?>();
        var pullingInterval = configuration.GetSection("EventSubscriber:PullingInterval").Get<int?>();
        var rabbitMqServer = configuration.GetSection("Infrastructures:RabbitMq:Server").Get<string?>();
        var rabbitMqPort = configuration.GetSection("Infrastructures:RabbitMq:Port").Get<ushort?>();
        var rabbitMqUserName = configuration.GetSection("Infrastructures:RabbitMq:UserName").Get<string?>();
        var rabbitMqPassword = configuration.GetSection("Infrastructures:RabbitMq:Password").Get<string?>();


        ArgumentNullException.ThrowIfNull(subscriptionName);
        ArgumentNullException.ThrowIfNull(eventDatabase);
        ArgumentNullException.ThrowIfNull(eventTable);
        ArgumentNullException.ThrowIfNull(cursorTable);
        ArgumentNullException.ThrowIfNull(pullingInterval);
        ArgumentNullException.ThrowIfNull(rabbitMqServer);
        ArgumentNullException.ThrowIfNull(rabbitMqPort);
        ArgumentNullException.ThrowIfNull(rabbitMqUserName);
        ArgumentNullException.ThrowIfNull(rabbitMqPassword);


        services.AddEventSubscriber(subscriptionName, a =>
            a.ReadFromSqlServer(c =>
            {
                c.ConnectionString = eventDatabase;
                c.EventTable = eventTable;
                c.CursorTable = cursorTable;
                c.PullingInterval = pullingInterval;
            })
                .PublishWithMassTransit(c =>
                {
                    c.RabbitMqConnectionString = rabbitMqServer;
                    c.Port = rabbitMqPort;
                    c.VirtualHost = "/";
                    c.UserName = rabbitMqUserName;
                    c.Password = rabbitMqPassword;
                })
                .UseEventsInAssemblies(typeof(SmsOtpRequested).Assembly)
                .UseEventTransformersInAssemblies()
                .WithNoFilter());

        return services;
    }
    public static IServiceCollection AddMessageInbox(this IServiceCollection services, IConfiguration configuration)
    {
        var tableName = configuration.GetSection("MessageInbox:TableName").Get<string?>();
        var connectionString = configuration.GetSection("Infrastructures:SqlServer").Get<string?>();


        var config = new SqlDuplicateHandlerConfig()
        {
            TableName = tableName,
            ConnectionString = connectionString
        };
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(config.TableName);
        ArgumentNullException.ThrowIfNull(config.ConnectionString);

        services.UseInboxPatternWithSqlServer(c =>
        {
            c.TableName = config.TableName;
            c.ConnectionString = config.ConnectionString;

        });

        return services;
    }
}