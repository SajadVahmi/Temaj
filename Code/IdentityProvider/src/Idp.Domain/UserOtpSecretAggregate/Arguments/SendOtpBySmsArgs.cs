using Framework.Core.Domain.Services;
using Idp.Domain._Shared.Contracts;
using Idp.Domain.UserAggregate.Contracts;

namespace Idp.Domain.UserOtpSecretAggregate.Arguments;

public record SendOtpBySmsArgs(ISmsSender SmsSender, IOtpService OtpService,IUserPhoneNumberResolver UsrPhoneNumberResolver ,IIdGenerator IdGenerator, IClock Clock);
