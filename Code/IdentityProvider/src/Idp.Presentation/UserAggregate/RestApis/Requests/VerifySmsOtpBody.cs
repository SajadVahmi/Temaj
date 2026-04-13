namespace Idp.Presentation.UserAggregate.RestApis.Requests;

public class VerifySmsOtpBody
{
    public string? PhoneNumber { get; set; }
    public string? OtpCode { get; set; }
}