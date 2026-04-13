using Framework.Core.Application.Commands;

namespace Idp.Application.UserOtpSecretAggregate.SendSmsOtp;

public class SendSmsOtpCommand : ICommand
{
    public long UserId { get; set; }
}