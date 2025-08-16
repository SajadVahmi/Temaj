using Framework.Core.Domain.Services;

namespace Idp.Domain.UserOtpSecretAggregate.Contracts;

public interface ISecretKeyGenerator:IDomainService
{
    string GenerateBase32Secret(int length = 20);
}