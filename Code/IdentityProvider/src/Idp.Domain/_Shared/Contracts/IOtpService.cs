using Framework.Core.Domain.Services;

namespace Idp.Domain._Shared.Contracts;

public interface IOtpService:IDomainService
{
    TimeSpan ExpirationTime => TimeSpan.FromMinutes(3);
    public string GenerateOtp(string base32Secret);
    public bool ValidateOtp(string base32Secret, string otp);
   
}