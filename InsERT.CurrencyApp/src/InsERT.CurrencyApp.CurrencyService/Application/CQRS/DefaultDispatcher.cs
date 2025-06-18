using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;

namespace InsERT.CurrencyApp.CurrencyService.Application.CQRS;

public class DefaultDispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResult>>();

        return handler is null
            ? throw new InvalidOperationException(
                $"No handler registered for command of type {typeof(TCommand).FullName}")
            : await handler.HandleAsync(command, cancellationToken);
    }
}
