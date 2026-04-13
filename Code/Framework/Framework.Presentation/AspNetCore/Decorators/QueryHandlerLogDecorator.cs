using Framework.Infrastructure.Queries;
using Microsoft.Extensions.Logging;

namespace Framework.Presentation.AspNetCore.Decorators;

public class QueryHandlerLogDecorator<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> inner,
    ILogger<IQueryHandler<TQuery, TResult>> logger)
    : IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult>
{


    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("{QueryHandler} started executing with query {@Query}", inner.GetType().Name, query);
            var result = await inner.HandleAsync(query, cancellationToken);
            logger.LogInformation("{QueryHandler} executed successfully with result {@Result}", inner.GetType().Name, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogInformation("{QueryHandler} goes with error", inner.GetType().Name);
            logger.LogError(ex.Message);
            throw;
        }
    }
}