using Framework.Core.Domain.Services;

namespace Idp.Domain.UserAggregate.Arguments;

public record RegisterUserArgs(
    string PhoneNumber,
    IIdGenerator IdGenerator,
    IClock Clock);