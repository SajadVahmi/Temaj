using Framework.Core.Application.Commands;

namespace Idp.Application.UserAggregate.SignInWithSmsOtp;

public class SignInWithSmsOtpCommand : ICommand
{
    public string PhoneNumber { get; set; } = null!;
    public string OtpCode { get; set; } = null!;
}