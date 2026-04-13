using System.Transactions;
using Framework.Core.Domain.DomainEvents;
using Framework.Core.Domain.Services;
using Framework.Infrastructure.EventProcessor.Idempotence;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Framework.Infrastructure.EventProcessor.EventBus.MassTransit;

public abstract class IdempotentMassTransitEventHandler<TEvent>(
    ILogger logger,
    IDuplicateEventHandler duplicateEventHandler,
    IJsonSerializerAdapter jsonSerializer,
    IClock clock)
    : EventHandler<TEvent>, IConsumer<TEvent>
    where TEvent : class, IEvent
{
    protected ConsumeContext<TEvent> ConsumeContext = null!;

    public async Task Consume(ConsumeContext<TEvent> context)
    {
        ConsumeContext=context;
        var consumerName = this.GetType().Name;

        logger.LogInformation("{consumerName} consuming {@message}", consumerName, context.Message);
        try
        {
            var message = context.Message;
            
            using (var scope = new TransactionScope(TransactionScopeOption.Required,new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!await duplicateEventHandler.HasMessageBeenProcessed(consumerName,message.EventId))
                {
                    await HandleEvent(message);

                    await duplicateEventHandler.MarkMessageAsProcessed(
                        messageId:message.EventId,
                        consumerName:consumerName,
                        typeName:typeof(TEvent).Name,
                        payload: jsonSerializer.Serialize(message)!,
                        receivedDateTime:clock.GetDateTime());

                    scope.Complete();
                }

            }

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
