using Idp.Domain._Shared.Contracts;
using OtpNet;

namespace Idp.Infrastructure.Services;

public class OtpNetService : IOtpService
{
    public TimeSpan ExpirationTime => TimeSpan.FromMinutes(3);

    public string GenerateOtp(string base32Secret)
    {
        var secretBytes = Base32Encoding.ToBytes(base32Secret);
        var totp = new Totp(secretBytes, step: (int)ExpirationTime.TotalSeconds);
        return totp.ComputeTotp();
    }

    public bool ValidateOtp(string base32Secret, string otp)
    {
        var secretBytes = Base32Encoding.ToBytes(base32Secret);
        var totp = new Totp(secretBytes, step: (int)ExpirationTime.TotalSeconds);

        bool isValid = totp.VerifyTotp(
            otp,
            out _,
            new VerificationWindow(previous: 1, future: 1)
        );

        return isValid;
    }
}