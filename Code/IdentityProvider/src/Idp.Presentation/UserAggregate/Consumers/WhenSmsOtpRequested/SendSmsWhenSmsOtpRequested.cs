using Framework.Core.Application.Commands;
using Framework.Core.Domain.Services;
using Framework.Infrastructure.EventProcessor.EventBus.MassTransit;
using Framework.Infrastructure.EventProcessor.Idempotence;
using Idp.Application.UserOtpSecretAggregate.SendSmsOtp;
using Idp.Domain.UserAggregate.Events;
using Microsoft.Extensions.Logging;

namespace Idp.Presentation.UserAggregate.Consumers.WhenSmsOtpRequested;

public class SendSmsWhenSmsOtpRequested(
    ILogger<SendSmsWhenSmsOtpRequested> logger,
    IDuplicateEventHandler duplicateEventHandler,
    IJsonSerializerAdapter jsonSerializer,
    IClock clock,
    ICommandBus commandBus) : IdempotentMassTransitEventHandler<SmsOtpRequested>(logger, duplicateEventHandler, jsonSerializer, clock)
{
    public override async Task HandleEvent(SmsOtpRequested @event)
    {
        var command = new SendSmsOtpCommand(){ UserId = @event.UserId};

        await commandBus.SendAsync(command);
    }
}