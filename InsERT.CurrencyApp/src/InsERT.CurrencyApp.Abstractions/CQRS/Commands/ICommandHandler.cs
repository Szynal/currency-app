namespace InsERT.CurrencyApp.Abstractions.CQRS.Commands;

/// <summary>
/// Handler for a specific command.
/// </summary>
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
