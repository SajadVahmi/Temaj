using Idp.Domain.UserAggregate;
using Idp.Domain.UserAggregate.Contracts;
using Idp.Infrastructure.Persistence._Shared;
using Microsoft.EntityFrameworkCore;

namespace Idp.Infrastructure.Persistence.UserAggregate;

public class UserRepository(IdpDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var dataModel = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return dataModel == null
            ? null
            : User.FromSnapshot(dataModel.GetSnapshot());
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var dataModel = new UserDataModel();
        dataModel.ApplySnapshot(user.GetSnapshot());

        await context.Users.AddAsync(dataModel, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var dataModel = context.Users.Local
                            .FirstOrDefault(u => u.Id == user.Id) ??
                        new UserDataModel();

        dataModel.ApplySnapshot(user.GetSnapshot());

        context.Users.Update(dataModel);
        return Task.CompletedTask;
    }
}