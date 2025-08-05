using Framework.Core.Domain.Services;

namespace Idp.Domain.UserAggregate.Arguments;

public record ConfirmEmailArgs(
    IIdGenerator IdGenerator,
    IClock Clock);