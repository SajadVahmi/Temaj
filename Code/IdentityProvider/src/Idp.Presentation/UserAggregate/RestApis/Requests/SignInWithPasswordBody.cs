namespace Idp.Presentation.UserAggregate.RestApis.Requests;

public class SignInWithPasswordBody
{
    public string? PhoneNumberOrEmail { get; set; }
    // Kept for backward compatibility with existing clients.
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
}
