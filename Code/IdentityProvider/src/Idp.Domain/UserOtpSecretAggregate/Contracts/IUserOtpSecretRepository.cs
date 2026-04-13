using Framework.Core.Domain.Repositories;
using Idp.Domain._Shared.Enums;

namespace Idp.Domain.UserOtpSecretAggregate.Contracts;

public interface IUserOtpSecretRepository:IRepository<UserOtpSecret,long>
{
    public Task<UserOtpSecret?> GetByChanelAsync(long userId,OtpChanel chanel ,CancellationToken cancellationToken = default);
    public void Add(UserOtpSecret userOtpSecret);
}