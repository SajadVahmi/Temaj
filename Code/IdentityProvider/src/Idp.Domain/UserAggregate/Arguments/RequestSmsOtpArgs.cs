using Framework.Core.Domain.Services;

namespace Idp.Domain.UserAggregate.Arguments;


public record RequestSmsOtpArgs(IIdGenerator IdGenerator, IClock Clock);