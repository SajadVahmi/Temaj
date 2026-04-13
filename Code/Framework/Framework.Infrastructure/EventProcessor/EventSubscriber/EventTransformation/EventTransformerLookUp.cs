using System.Reflection;
using Framework.Core.Domain.DomainEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.EventTransformation;

public class EventTransformerLookUp : IEventTransformerLookUp
{
    private readonly Dictionary<string, Type> _transformers = new Dictionary<string, Type>();
    public void AddTypesFromAssembly(Assembly assembly,IServiceCollection serviceCollection,string subscriberName)
    {
        var events = assembly.GetTypes()
            .Where(IsImplementationOfEventTransformer)
            .ToList();

        events.ForEach(transformer =>
        {
            var typeOfEvent = transformer.BaseType?.GetGenericArguments().First();
            if (typeOfEvent != null)
            {
                _transformers.Add(typeOfEvent.Name, transformer);
                serviceCollection.AddKeyedSingleton(serviceType:transformer, subscriberName);
            }
               

        });
    }

    private static bool IsImplementationOfEventTransformer(Type type)
    {
        return type.BaseType is { IsGenericType: true } &&
               type.BaseType.GetGenericTypeDefinition() == typeof(EventTransformer<>);
    }

    public IEventTransformer? LookUpTransformer(IEvent @event)
    {
        var nameOfEvent = @event.GetType().Name;
        if (!_transformers.TryGetValue(nameOfEvent, out var transformer)) return null;
        return Activator.CreateInstance(transformer) as IEventTransformer;
    }

    public Type? LookUpTransformerType(IEvent @event)
    {
        var nameOfEvent = @event.GetType().Name;
        return !_transformers.TryGetValue(nameOfEvent, out var transformer) ? null : transformer;
    }
}