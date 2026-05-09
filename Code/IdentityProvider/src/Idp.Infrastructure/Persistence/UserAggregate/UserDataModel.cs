using Framework.Core.Domain.Aggregates;
using Framework.Core.Domain.DomainEvents;
using Idp.Domain.UserAggregate.Snapshots;
using Microsoft.AspNetCore.Identity;

namespace Idp.Infrastructure.Persistence.UserAggregate;

public class UserDataModel :IdentityUser<long>,IAggregateRoot
{
    private readonly List<IDomainEvent> _events = [];
    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);
    public IEnumerable<IDomainEvent> GetEvents() => _events.AsEnumerable();
    public void ClearEvents() => _events.Clear();

    public DateTimeOffset? AuthenticatorTwoFactorEnabledAt { get; set; }

    public UserSnapshot GetSnapshot()
    {
        return new UserSnapshot
        (
            Id : Id,
            Email : Email,
            IsEmailConfirmed : EmailConfirmed,
            PhoneNumber : PhoneNumber!,
            IsPhoneNumberConfirmed : PhoneNumberConfirmed
        );
    }

    public void ApplySnapshot(UserSnapshot snapshot)
    {
        Id = snapshot.Id;
        UserName = snapshot.PhoneNumber;
        NormalizedUserName = snapshot.PhoneNumber;
        Email = snapshot.Email;
        NormalizedEmail = snapshot.Email?.ToUpperInvariant();
        EmailConfirmed = snapshot.IsEmailConfirmed;
        PhoneNumber = snapshot.PhoneNumber;
        PhoneNumberConfirmed = snapshot.IsPhoneNumberConfirmed;
    }

    public void ApplyEvents(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var @event in domainEvents)
            AddEvent(@event);
    }

}
