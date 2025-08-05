using Framework.Core.Domain.Repositories;

namespace Idp.Domain.UserAggregate.Repositories;

public interface IUserRepository : IRepository<User, long>
{
    public Task RegisterAsync(User user, CancellationToken cancellationToken = default);

    public Task ConfirmEmailAsync(User user, CancellationToken cancellationToken = default);

    public Task ConfirmPhoneNumberAsync(User user, CancellationToken cancellationToken = default);

}