using Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore;
using Framework.Infrastructure.EventProcessor.EventSubscriber.EventProcessingActions;
using Framework.Infrastructure.EventProcessor.EventSubscriber.EventTransformation;
using Framework.Infrastructure.EventProcessor.EventSubscriber.Filtering;
using Framework.Infrastructure.EventProcessor.EventSubscriber.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.Setup
{
    public static class EventSubscriberServiceExtensions
    {
        public static void AddEventSubscriber(this IServiceCollection services, string subscriptionName, Action<EventSubscriberConfigurator> config)
        {

            var configurator = new EventSubscriberConfigurator(services, subscriptionName);
            config.Invoke(configurator);

            var serviceProvider = services.BuildServiceProvider();

            var worker = new EventSubscriberWorker(
                subscriberName: subscriptionName,
                logger: serviceProvider.GetRequiredService<ILogger<EventSubscriberWorker>>(),
                eventStoreSubscriber: serviceProvider.GetRequiredKeyedService<IEventStoreSubscriber>(
                    subscriptionName),
                serviceProvider: serviceProvider,
                eventProcessor: serviceProvider.GetRequiredKeyedService<IEventProcessor>(subscriptionName),
                typeResolver: serviceProvider.GetRequiredKeyedService<IEventTypeResolver>(subscriptionName),
                filter: serviceProvider.GetRequiredKeyedService<IFilter>(subscriptionName),
                transformerLookUp: serviceProvider.GetRequiredKeyedService<IEventTransformerLookUp>(
                    subscriptionName));


            services.AddSingleton<IHostedService>(worker);

        }
    }
}