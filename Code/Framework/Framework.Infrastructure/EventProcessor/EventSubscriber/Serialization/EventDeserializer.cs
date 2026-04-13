using Framework.Core.Domain.DomainEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.Serialization;

public static class EventDeserializer
{
    private static readonly JsonSerializerSettings Settings;
    static EventDeserializer()
    {
        Settings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterContractResolver(),
        };
    }
    public static IEvent Deserialize(Type type, string body)
    {
        return (JsonConvert.DeserializeObject(body, type, Settings) as IEvent)!;
    }
}