namespace Idp.Presentation.UserAggregate.RestApis.Requests;

public class SignInWithAuthenticatorCodeBody
{
    public string? VerificationCode { get; set; }
    public bool RememberMachine { get; set; }
}
