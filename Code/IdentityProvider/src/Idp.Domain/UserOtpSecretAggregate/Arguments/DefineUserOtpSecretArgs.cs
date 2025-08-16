using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Enums;
using Idp.Domain.UserOtpSecretAggregate.Contracts;

namespace Idp.Domain.UserOtpSecretAggregate.Arguments;

public record DefineUserOtpSecretArgs(
    long UserId,
    OtpChanel Chanel,
    ISecretKeyGenerator SecretKeyGenerator,
    IIdGenerator IdGenerator,
    IClock Clock);