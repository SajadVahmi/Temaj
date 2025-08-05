using Framework.Core.Domain.Services;

namespace Idp.Domain.UserAggregate.Arguments;

public record ConfirmPhoneNumberArgs(
    IIdGenerator IdGenerator,
    IClock Clock);