using System.Reflection;

namespace Framework.Infrastructure.EventProcessor.EventSubscriber.Types
{
    public interface IEventTypeResolver
    {
        void AddTypesFromAssembly(Assembly assembly);
        Type? GetType(string typeName);
    }
}