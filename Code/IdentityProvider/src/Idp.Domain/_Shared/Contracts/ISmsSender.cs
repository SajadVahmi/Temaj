using Framework.Core.Domain.Services;

namespace Idp.Domain._Shared.Contracts;

public interface ISmsSender : IDomainService
{
    public Task SendSmsAsync(
        string to,
        string message,
        string? from = null,
        CancellationToken cancellationToken = default);
}