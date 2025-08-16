using Idp.Domain.UserOtpSecretAggregate;
using Idp.Domain.UserOtpSecretAggregate.Contracts;
using Idp.Infrastructure.Persistence._Shared;
using Microsoft.EntityFrameworkCore;

namespace Idp.Infrastructure.Persistence.UserOtpSecretAggregate;

public class UserOtpSecretRepository(IdpDbContext dbContext):IUserOtpSecretRepository
{
    public Task<UserOtpSecret?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return dbContext.Set<UserOtpSecret>().FirstOrDefaultAsync(us => us.Id == id, cancellationToken);
    }

    public void Add(UserOtpSecret userOtpSecret)
    {
        dbContext.Set<UserOtpSecret>().Add(userOtpSecret);
    }
}