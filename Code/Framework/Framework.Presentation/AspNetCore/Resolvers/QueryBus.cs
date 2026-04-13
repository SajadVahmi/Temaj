using Framework.Infrastructure.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Presentation.AspNetCore.Resolvers;

public class QueryBus(IServiceProvider serviceProvider) : IQueryBus
{
    public  Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken) 
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

        var handler = serviceProvider.GetRequiredService(handlerType);

        var methodName = nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync);

        var handleMethod = handlerType.GetMethod(methodName, new[] { query.GetType(), typeof(CancellationToken) });

        if (handleMethod == null)
        {
            throw new InvalidOperationException($"The handler does not have a method named 'HandleAsync' that accepts the query type '{query.GetType().Name}' and a CancellationToken.");
        }

        return (Task<TResult>)handleMethod.Invoke(handler, new object?[] { query, cancellationToken })!;

    }

   
}