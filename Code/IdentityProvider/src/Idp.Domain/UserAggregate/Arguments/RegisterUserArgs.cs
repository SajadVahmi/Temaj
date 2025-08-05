using Framework.Core.Domain.Services;

namespace Idp.Domain.UserAggregate.Arguments;

public record RegisterUserArgs(
    string UserName,
    IIdGenerator IdGenerator,
    IClock Clock);