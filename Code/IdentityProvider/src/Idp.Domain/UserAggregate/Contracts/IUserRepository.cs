using Framework.Core.Domain.Repositories;

namespace Idp.Domain.UserAggregate.Contracts;

public interface IUserRepository : IRepository<User, long>
{
    public Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}