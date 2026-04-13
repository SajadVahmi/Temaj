using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Contracts;
using Idp.Domain.UserOtpSecretAggregate.Contracts;

namespace Idp.Domain.UserAggregate.Arguments;

public record SignInWithSmsOtpArgs(
    string OtpCode,
    IUserOtpSecretRepository OtpSecretRepository,
    IOtpService OtpService,
    ISignInService SignInService,
    IIdGenerator IdGenerator,
    IClock Clock);