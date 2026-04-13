using Framework.Core.Domain.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Framework.Infrastructure.Tools.JsonSerializers.NewtonSoft;

public class NewtonSoftSerializerAdapter(JsonSerializerSettings settings) : IJsonSerializerAdapter
{
    public NewtonSoftSerializerAdapter() : this(new JsonSerializerSettings()
    {
        ContractResolver = new PrivateSetterContractResolver(),
    })
    {
    }

    public TOutput? Deserialize<TOutput>(string input)
    {
        return string.IsNullOrWhiteSpace(input) ? default : JsonConvert.DeserializeObject<TOutput>(input, settings);
    }

    public object? Deserialize(string input, Type type)
    {
        return string.IsNullOrWhiteSpace(input) ? default : JsonConvert.DeserializeObject(input, type, settings);
    }

    public string? Serialize<TInput>(TInput input)
    {
        return input == null ? null : JsonConvert.SerializeObject(input, settings);
    }
}
