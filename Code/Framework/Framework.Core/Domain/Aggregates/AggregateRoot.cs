using Framework.Core.Domain.DomainEvents;
using Framework.Core.Domain.Entities;

namespace Framework.Core.Domain.Aggregates;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : notnull
{

    private readonly List<IDomainEvent> _events = [];

    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);


    public IEnumerable<IDomainEvent> GetEvents() => _events.AsEnumerable();

    public void ClearEvents() => _events.Clear();

    protected abstract void CheckInvariants();
}