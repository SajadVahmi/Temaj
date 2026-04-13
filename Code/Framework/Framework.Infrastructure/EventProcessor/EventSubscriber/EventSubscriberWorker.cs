using Framework.Core.Domain.DomainEvents;
using Framework.Infrastructure.EventProcessor.EventSubscriber.DataStore;
using Framework.Infrastructure.EventProcessor.EventSubscriber.EventProcessingActions;
using Framework.Infrastructure.EventProcessor.EventSubscriber.EventTransformation;
using Framework.Infrastructure.EventProcessor.EventSubscriber.Filtering;
using Framework.Infrastructure.EventProcessor.EventSubscriber.Serialization;
using Framework.Infrastructure.EventProcessor.EventSubscriber.Types;
using Framework.Infrastructure.Persistence.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber
{
    public class EventSubscriberWorker : BackgroundService, IEventStoreChangeTracker
    {
        private readonly string _subscriberName;
        private readonly ILogger<EventSubscriberWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventStoreSubscriber _eventStoreSubscriber;
        private readonly IEventProcessor _eventProcessor;
        private readonly IEventTypeResolver _typeResolver;
        private readonly IFilter _filter;
        private readonly IEventTransformerLookUp _transformerLookUp;

        public EventSubscriberWorker(
            string subscriberName,
            ILogger<EventSubscriberWorker> logger,
            IEventStoreSubscriber eventStoreSubscriber,
            IEventProcessor eventProcessor, 
            IEventTypeResolver typeResolver,
            IFilter filter,
            IEventTransformerLookUp transformerLookUp, IServiceProvider serviceProvider)
        {
            _subscriberName = subscriberName;
            _logger = logger;
            _eventStoreSubscriber = eventStoreSubscriber;
            _eventStoreSubscriber.SetSubscriber(this);
            _eventProcessor = eventProcessor;
            _typeResolver = typeResolver;
            _filter = filter;
            _transformerLookUp = transformerLookUp;
            _serviceProvider = serviceProvider;
        }

        public async Task RunAsync(CancellationToken stoppingToken)
        {
            await ExecuteAsync(stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
             await _eventProcessor.StartAsync(stoppingToken);
            _logger.LogInformation("{SubscriberName} started.", _subscriberName);
            _eventStoreSubscriber.SubscribeForChanges();
            _logger.LogInformation("{SubscriberName} subscribed to event store", _subscriberName);
        }

        public async Task ChangeDetected(List<EventItem> items)
        {
            foreach (var item in items)
            {
                var type = _typeResolver.GetType(item.EventName!);
                if (type is null)
                {
                    _logger.LogError($"Type of '{item.EventName}' not found in event types for {_subscriberName}");
                    continue;
                }

                var eventToPublish = (dynamic)EventDeserializer.Deserialize(type, item.EventPayload!);

                if (_filter.ShouldProcess(eventToPublish))
                {
                    eventToPublish = TransformEvent(eventToPublish, item);
                    await _eventProcessor.ProcessAsync(eventToPublish);
                    _logger.LogInformation($"Event '{item.EventTypeName}-{item.EventId}' processed in {_subscriberName}.");
                }
                else
                {
                    _logger.LogInformation($"'{item.EventTypeName}-{item.EventId}' processing skipped in {_subscriberName} because of filter");
                }
            }
        }

        private object TransformEvent(IEvent eventToPublish, EventItem item)
        {
            var transformerType = _transformerLookUp.LookUpTransformerType(eventToPublish);
            if (transformerType != null)
            {
                var transformer = _serviceProvider.GetRequiredKeyedService(transformerType, _subscriberName) as IEventTransformer;
                if(transformer==null)
                    return eventToPublish;

                _logger.LogInformation($"'{item.EventTypeName}-{item.EventId}' transformed in {_subscriberName}");
                return transformer.TransformEvent(eventToPublish);
               
            }
            return eventToPublish;
        }
    }
}
