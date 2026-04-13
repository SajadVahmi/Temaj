using Idp.Domain._Shared.Contracts;

namespace Idp.Infrastructure.Services;

public class SmsSenderService: ISmsSender
{
    public Task SendSmsAsync(string to, string message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}