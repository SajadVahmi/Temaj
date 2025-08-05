namespace Framework.Core.Domain.Services;

public interface IJsonSerializerAdapter
{
    string? Serialize<TInput>(TInput input);

    TOutput? Deserialize<TOutput>(string input);

    object? Deserialize(string input, Type type);
}