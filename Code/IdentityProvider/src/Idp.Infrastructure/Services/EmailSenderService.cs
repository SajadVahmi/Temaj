using Idp.Domain._Shared.Contracts;

namespace Idp.Infrastructure.Services;

public class EmailSenderService:IEmailSender
{
    public Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true, string? from = null,
        IEnumerable<string>? cc = null, IEnumerable<string>? bcc = null, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}