namespace Framework.Infrastructure.Queries;

public interface IQueryHandlerResolver
{
    object ResolveHandlers<TQuery, TResult>(TQuery request) where TQuery : class, IQuery<TResult>;
}