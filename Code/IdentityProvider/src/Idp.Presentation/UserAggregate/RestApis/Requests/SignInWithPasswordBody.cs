namespace Idp.Presentation.UserAggregate.RestApis.Requests;

public class SignInWithPasswordBody
{
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
}
