using InsERT.CurrencyApp.Abstractions.CQRS.Commands;

namespace InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;

/// <summary>
/// Dispatcher for executing commands (or queries in the future).
/// </summary>
public interface IDispatcher
{
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;
}
