using Idp.Domain.UserAggregate.Contracts;
using Idp.Infrastructure.Persistence._Shared;
using Microsoft.EntityFrameworkCore;

namespace Idp.Infrastructure.Persistence.UserAggregate;

public class UserPhoneNumberResolver(IdpDbContext dbContext) : IUserPhoneNumberResolver
{
    public async Task<string?> GetUserPhoneNumberAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user?.PhoneNumber;
    }
}