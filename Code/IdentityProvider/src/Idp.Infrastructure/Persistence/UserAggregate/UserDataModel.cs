using Idp.Domain.UserAggregate.Snapshots;
using Microsoft.AspNetCore.Identity;

namespace Idp.Infrastructure.Persistence.UserAggregate;

public class UserDataModel :IdentityUser<long>
{
    public UserSnapshot GetSnapshot()
    {
        return new UserSnapshot
        (
            Id : Id,
            Email : Email,
            IsEmailConfirmed : EmailConfirmed,
            PhoneNumber : PhoneNumber,
            IsPhoneNumberConfirmed : PhoneNumberConfirmed
        );
    }

    public void ApplySnapshot(UserSnapshot snapshot)
    {
        Id = snapshot.Id;
        Email = snapshot.Email;
        EmailConfirmed = snapshot.IsEmailConfirmed;
        PhoneNumber = snapshot.PhoneNumber;
        PhoneNumberConfirmed = snapshot.IsPhoneNumberConfirmed;
    }
}
