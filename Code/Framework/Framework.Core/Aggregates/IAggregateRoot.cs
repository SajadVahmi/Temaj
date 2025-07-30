using Framework.Core.DomainEvents;
using Framework.Core.Entities;

namespace Framework.Core.Aggregates;

public interface IAggregateRoot<out TId> : IEntity<TId>
{
    void ClearEvents();
    IEnumerable<IDomainEvent> GetEvents();

}