using Framework.Core.Application.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Presentation.AspNetCore.Resolvers;

public class CommandBus(IServiceProvider serviceProvider) : ICommandBus
{
    public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class, ICommand
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        return handler.HandleAsync(command, cancellationToken);
    }

    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

        var handler = serviceProvider.GetRequiredService(handlerType);

        var methodName = nameof(ICommandHandler<ICommand<TResult>, TResult>.HandleAsync);

        var handleMethod = handlerType.GetMethod(methodName, new[] { command.GetType(), typeof(CancellationToken) });

        if (handleMethod == null)
        {
            throw new InvalidOperationException($"The handler does not have a method named 'HandleAsync' that accepts the query type '{command.GetType().Name}' and a CancellationToken.");
        }

        return (Task<TResult>)handleMethod.Invoke(handler, new object?[] { command, cancellationToken })!;

        
    }

    
}