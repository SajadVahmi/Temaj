using Framework.Core.Application.Commands;

namespace Idp.Application.UserAggregate.RequestSmsOtp;

public class RequestSmsOtpCommand : ICommand
{
    public string PhoneNumber { get; set; } = null!;

}