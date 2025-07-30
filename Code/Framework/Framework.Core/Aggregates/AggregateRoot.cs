using Framework.Core.DomainEvents;
using Framework.Core.Entities;

namespace Framework.Core.Aggregates;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId> where TId : notnull
{

    private readonly List<IDomainEvent> _events = [];

    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);


    public IEnumerable<IDomainEvent> GetEvents() => _events.AsEnumerable();

    public void ClearEvents() => _events.Clear();
}