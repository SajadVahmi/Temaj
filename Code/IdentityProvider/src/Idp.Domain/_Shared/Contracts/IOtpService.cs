using Framework.Core.Domain.Services;

namespace Idp.Domain._Shared.Contracts;

public interface IOtpService:IDomainService
{
    public Task<string> GenerateOtpAsync(string userId,CancellationToken cancellationToken=default);
    public Task<bool> ValidateOtpAsync(string userId, string otp,CancellationToken cancellationToken=default);
}