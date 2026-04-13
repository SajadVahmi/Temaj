using Framework.Core.Domain.Repositories;

namespace Idp.Infrastructure.Persistence._Shared;

public class UnitOfWork(IdpDbContext dbContext):IUnitOfWork 
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}