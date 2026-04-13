using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Framework.Infrastructure.EventProcessor.EventBus.MassTransit
{
    public class MassTransitConsoleHostedService(IBusControl bus, ILoggerFactory loggerFactory) :
        IHostedService
    {
        readonly ILogger _logger = loggerFactory.CreateLogger<MassTransitConsoleHostedService>();
        private Task? _executingTask;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bus");
            _executingTask = bus.StartAsync(cancellationToken);

            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping bus");
            return bus.StopAsync(cancellationToken);
        }
    }
}
