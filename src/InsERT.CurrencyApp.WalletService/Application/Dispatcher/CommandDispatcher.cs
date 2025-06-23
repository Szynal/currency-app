using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.WalletService.Application.Behaviors;

namespace InsERT.CurrencyApp.WalletService.Application.Dispatcher;

public sealed class CommandDispatcher(IServiceProvider serviceProvider, IValidationBehavior validator) : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IValidationBehavior _validator = validator;

    public async Task<TResult> SendAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        await _validator.ValidateAsync(command, cancellationToken);

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();

        return await handler.HandleAsync(command, cancellationToken);
    }
}
