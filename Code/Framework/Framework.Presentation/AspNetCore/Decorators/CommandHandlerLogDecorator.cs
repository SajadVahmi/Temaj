using Framework.Core.Application.Commands;
using Microsoft.Extensions.Logging;

namespace Framework.Presentation.AspNetCore.Decorators;

public class CommandHandlerLogDecorator<TCommand>(
    ICommandHandler<TCommand> handler,
    ILogger<ICommandHandler<TCommand>> logger)
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("{CommandHandler} started executing with command {@Command}", handler.GetType().Name, command);
            await handler.HandleAsync(command, cancellationToken);
            logger.LogInformation("{CommandHandler} executed successfully", handler.GetType().Name);
        }
        catch (Exception ex)
        {
            logger.LogInformation("{CommandHandler} goes with error", handler.GetType().Name);
            logger.LogError(ex.Message);
            throw;
        }
    }
}


public class CommandHandlerLogDecorator<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> handler,
    ILogger<ICommandHandler<TCommand, TResult>> logger)
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public async Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("{CommandHandler} started executing with command {@Command}", handler.GetType().Name, command);
            var result = await handler.HandleAsync(command, cancellationToken);
            logger.LogInformation("{CommandHandler} executed successfully with result {@Result}", handler.GetType().Name, result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogInformation("{CommandHandler} goes with error", handler.GetType().Name);
            logger.LogError(ex.Message);
            throw;
        }
    }
}
