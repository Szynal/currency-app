using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;

namespace InsERT.CurrencyApp.TransactionService.Application.Commands.Handlers;

public class CreateConversionCommandHandler(
    ICommandDispatcher dispatcher,
    ILogger<CreateConversionCommandHandler> logger
) : ICommandHandler<CreateConversionCommand, Unit>
{
    public async Task<Unit> HandleAsync(CreateConversionCommand command, CancellationToken cancellationToken = default)
    {
        var applyCommand = new ApplyTransactionCommand(
            WalletId: command.WalletId,
            Type: Domain.Entities.TransactionType.ConvertCurrency,
            Amount: command.SourceAmount,
            CurrencyCode: command.SourceCurrencyCode,
            ConvertedCurrencyCode: command.TargetCurrencyCode
        );

        await dispatcher.SendAsync<ApplyTransactionCommand, Unit>(applyCommand, cancellationToken);

        logger.LogInformation(
            "CreateConversionCommand handled for WalletId {WalletId}: {SourceAmount} {SourceCurrency} -> {TargetCurrency}",
            command.WalletId,
            command.SourceAmount,
            command.SourceCurrencyCode,
            command.TargetCurrencyCode
        );

        return Unit.Value;
    }
}
