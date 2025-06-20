using InsERT.CurrencyApp.Abstractions.CQRS.Commands;

namespace InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;

public interface ICommandDispatcher
{
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;
}
