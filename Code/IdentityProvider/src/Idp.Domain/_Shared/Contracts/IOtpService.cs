using Framework.Core.Domain.Services;

namespace Idp.Domain._Shared.Contracts;

public interface IOtpService:IDomainService
{
    TimeSpan ExpirationTime => TimeSpan.FromMinutes(3);
    public string GenerateOtp();
    public Task<bool> ValidateOtpAsync(string userId, string otp,CancellationToken cancellationToken=default);
   
}