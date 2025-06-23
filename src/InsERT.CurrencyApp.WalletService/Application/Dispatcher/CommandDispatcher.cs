using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.WalletService.Application.Behaviors;

namespace InsERT.CurrencyApp.WalletService.Application.Dispatcher;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IValidationBehavior _validator;

    public CommandDispatcher(IServiceProvider serviceProvider, IValidationBehavior validator)
    {
        _serviceProvider = serviceProvider;
        _validator = validator;
    }

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
