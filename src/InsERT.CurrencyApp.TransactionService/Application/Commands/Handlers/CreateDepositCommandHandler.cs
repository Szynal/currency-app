using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;


namespace InsERT.CurrencyApp.TransactionService.Application.Commands.Handlers;

public class CreateDepositCommandHandler(
    ICommandDispatcher commandDispatcher,
    ILogger<CreateDepositCommandHandler> logger
) : ICommandHandler<CreateDepositCommand, Unit>
{
    public async Task<Unit> HandleAsync(CreateDepositCommand command, CancellationToken cancellationToken = default)
    {
        var internalCommand = new ApplyTransactionCommand(
            WalletId: command.WalletId,
            Type: Domain.Entities.TransactionType.Deposit,
            Amount: command.Amount,
            CurrencyCode: command.CurrencyCode
        );

        logger.LogInformation("Dispatching internal ApplyTransactionCommand for deposit. WalletId={WalletId}, Amount={Amount}, Currency={Currency}",
            command.WalletId, command.Amount, command.CurrencyCode);

        await commandDispatcher.SendAsync<ApplyTransactionCommand, Unit>(internalCommand, cancellationToken);

        return Unit.Value;
    }
}
