using Idp.Domain._Shared.Contracts;
using Idp.Domain.UserOtpSecretAggregate.Contracts;
using OtpNet;

namespace Idp.Infrastructure.Services;

public class OtpNetSecretKeyGenerator : ISecretKeyGenerator
{
    public string GenerateBase32Secret(int length = 20)
    {
        var key = KeyGeneration.GenerateRandomKey(length);

        return Base32Encoding.ToString(key);
    }
}