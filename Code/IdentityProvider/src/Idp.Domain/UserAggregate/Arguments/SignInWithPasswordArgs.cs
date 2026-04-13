using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Contracts;

namespace Idp.Domain.UserAggregate.Arguments;

public record SignInWithPasswordArgs(
    string Password,
    ISignInService SignInService,
    IIdGenerator IdGenerator,
    IClock Clock);
