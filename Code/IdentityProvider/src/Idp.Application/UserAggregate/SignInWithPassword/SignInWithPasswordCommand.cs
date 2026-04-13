using Framework.Core.Application.Commands;

namespace Idp.Application.UserAggregate.SignInWithPassword;

public class SignInWithPasswordCommand : ICommand
{
    public string PhoneNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
}
