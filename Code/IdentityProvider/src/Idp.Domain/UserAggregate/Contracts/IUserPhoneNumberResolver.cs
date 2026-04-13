using Framework.Core.Domain.Services;

namespace Idp.Domain.UserAggregate.Contracts;

public interface IUserPhoneNumberResolver: IDomainService
{
    public Task<string?> GetUserPhoneNumberAsync(long userId, CancellationToken cancellationToken = default);
}