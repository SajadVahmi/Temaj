using Framework.Core.Domain.DomainEvents;
using Framework.Core.Domain.Entities;

namespace Framework.Core.Domain.Aggregates;

public interface IAggregateRoot
{
    void ClearEvents();
    IEnumerable<IDomainEvent> GetEvents();
};
