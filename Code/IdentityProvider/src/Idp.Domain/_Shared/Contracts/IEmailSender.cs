using Framework.Core.Domain.Services;

namespace Idp.Domain._Shared.Contracts;

public interface IEmailSender : IDomainService
{
    public Task SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isBodyHtml = true,
        string? from = null,
        IEnumerable<string>? cc = null,
        IEnumerable<string>? bcc = null,
        CancellationToken cancellationToken=default);
}