using Framework.Core.Domain.Repositories;

namespace Idp.Domain.UserOtpSecretAggregate.Contracts;

public interface IUserOtpSecretRepository:IRepository<UserOtpSecret,long>
{
    public void Add(UserOtpSecret userOtpSecret);
}