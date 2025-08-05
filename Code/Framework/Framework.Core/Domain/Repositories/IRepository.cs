using Framework.Core.Domain.Aggregates;

namespace Framework.Core.Domain.Repositories;

public interface IRepository<TAggregate, in TKey> where TAggregate : AggregateRoot<TKey> where TKey : notnull
{
    public Task<TAggregate?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    
}