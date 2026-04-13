using Framework.Core.Domain.DomainEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Framework.Infrastructure.EventProcessor.EventBus.MassTransit;

public abstract class MassTransitHandler<TEvent>(ILogger logger) : EventHandler<TEvent>, IConsumer<TEvent>
    where TEvent : class, IEvent
{
    public async Task Consume(ConsumeContext<TEvent> context)
    {
        logger.LogInformation("{consumerName} consuming {@message}", this.GetType().Name, context.Message);
        try
        {
            await HandleEvent(context.Message);

            logger.LogInformation("{consumerName} consumed {@message}", this.GetType().Name, context.Message);
        }
        catch (Exception e)
        {
            logger.LogInformation("{consumerName} goes with error for consuming {@message}", this.GetType().Name, context.Message);
            logger.LogError(e.Message);
            throw;
        }
    }
}