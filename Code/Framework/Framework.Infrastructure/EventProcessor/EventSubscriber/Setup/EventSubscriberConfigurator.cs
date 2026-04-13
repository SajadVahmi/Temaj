using System.Reflection;
using Framework.Infrastructure.EventProcessor.EventBus.MassTransit;
using Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore;
using Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore.SqlServer;
using Framework.Infrastructure.EventProcessor.EventSubscriber.EventProcessingActions;
using Framework.Infrastructure.EventProcessor.EventSubscriber.EventTransformation;
using Framework.Infrastructure.EventProcessor.EventSubscriber.Filtering;
using Framework.Infrastructure.EventProcessor.EventSubscriber.Types;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.Setup;

public class EventSubscriberConfigurator(IServiceCollection serviceCollection,string subscriptionName)
{
    public EventSubscriberConfigurator ReadFromSqlServer(Action<SqlServerEventStoreSubscriberConfig> config)
    {

        var subscriberConfig = new SqlServerEventStoreSubscriberConfig();

        config.Invoke(subscriberConfig);

        
        ArgumentNullException.ThrowIfNull(subscriptionName);
        ArgumentNullException.ThrowIfNull(subscriberConfig);
        ArgumentNullException.ThrowIfNull(subscriberConfig.CursorTable);
        ArgumentNullException.ThrowIfNull(subscriberConfig.PullingInterval);
        ArgumentNullException.ThrowIfNull(subscriberConfig.ConnectionString);


        serviceCollection.AddKeyedSingleton(subscriberConfig, subscriptionName);

        using var connection = new SqlConnection(subscriberConfig.ConnectionString);

        connection.CreateCursorTableIfNotExist(subscriberConfig, subscriptionName);

        serviceCollection.AddKeyedSingleton<IEventStoreSubscriber>(subscriptionName, (provider, o) => new SqlServerEventStoreSubscriber(
            subscriptionName: subscriptionName,
            sqlStoreConfig: subscriberConfig,
            logger: provider.GetRequiredService<ILogger<SqlServerEventStoreSubscriber>>()));
        return this;
         
    }
    public EventSubscriberConfigurator ProcessEventsWith(IEventProcessor eventProcessor)
    {


        serviceCollection.AddKeyedSingleton(subscriptionName, (provider, o) => eventProcessor);

        return this;
    }
    public EventSubscriberConfigurator PublishWithMassTransit(Action<MassTransitPublishOptions> config)
    {
        var massTransitConfig = new MassTransitPublishOptions();

        config.Invoke(massTransitConfig);

        serviceCollection.AddKeyedSingleton<IEventProcessor>(subscriptionName,(provider,o) => 
            new MasstransitEventPublisher(publishOptions: massTransitConfig));

        return this;
    }
    public EventSubscriberConfigurator SendWithMassTransit(Action<MassTransitSendOptions> config)
    {
        var massTransitConfig = new MassTransitSendOptions();

        config.Invoke(massTransitConfig);

        serviceCollection.AddKeyedSingleton<IEventProcessor>(subscriptionName, (provider, o) => new MasstransitEventSender(
            sendOptions: massTransitConfig
        ));

        return this;
    }
    public EventSubscriberConfigurator WithFilter(IFilter filter)
    {
        serviceCollection.AddKeyedSingleton(subscriptionName,filter);
        return this;
    }
    public EventSubscriberConfigurator WithNoFilter()
    {
        return WithFilter(new NoFilter());
    }
    public EventSubscriberConfigurator UseEventsInAssemblies(params Assembly[] assemblies)
    {
        var eventTypeResolver = new EventTypeResolver();
        if (assemblies.Length > 0)
        {
            foreach (var assembly in assemblies)
                eventTypeResolver.AddTypesFromAssembly(assembly);
        }
        serviceCollection.AddKeyedSingleton<IEventTypeResolver>(subscriptionName,eventTypeResolver);
        return this;
    }
    public EventSubscriberConfigurator UseEventTransformersInAssemblies(params Assembly[] assemblies)
    {
        var transformerLookUp = new EventTransformerLookUp();
        if(assemblies.Length > 0)
        {
            foreach (var assembly in assemblies)
                transformerLookUp.AddTypesFromAssembly(assembly,serviceCollection,subscriptionName);
        }
        serviceCollection.AddKeyedSingleton<IEventTransformerLookUp>(subscriptionName,transformerLookUp);
        return this;
    }
    public EventSubscriberConfigurator WithNoEventTransformer()
    {
        return UseEventTransformersInAssemblies();
    }
}