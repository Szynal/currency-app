using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.Abstractions.Currency.Models;
using InsERT.CurrencyApp.Abstractions.Currency.Queries;
using InsERT.CurrencyApp.TransactionService.Domain;
using InsERT.CurrencyApp.TransactionService.Domain.Entities;
using InsERT.CurrencyApp.TransactionService.Domain.Repositories;
using InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;

namespace InsERT.CurrencyApp.TransactionService.Application.Commands.Handlers;

public class ApplyTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IWalletServiceClient walletServiceClient,
    IUnitOfWork unitOfWork,
    IQueryDispatcher queryDispatcher,
    ILogger<ApplyTransactionCommandHandler> logger) : ICommandHandler<ApplyTransactionCommand, Unit>
{
    public async Task<Unit> HandleAsync(ApplyTransactionCommand command, CancellationToken cancellationToken = default)
    {
        Transaction transaction = command.Type switch
        {
            TransactionType.Deposit => Transaction.CreateDeposit(command.WalletId, command.CurrencyCode, command.Amount),
            TransactionType.Withdraw => Transaction.CreateWithdrawal(command.WalletId, command.CurrencyCode, command.Amount),
            TransactionType.ConvertCurrency => await CreateConversionAsync(command, cancellationToken),
            _ => throw new InvalidOperationException("Unsupported transaction type")
        };

        try
        {
            await transactionRepository.AddAsync(transaction, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await walletServiceClient.ApplyTransactionAsync(transaction, cancellationToken);

            transaction.MarkAccepted();
            await transactionRepository.UpdateAsync(transaction, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Transaction {TransactionId} processed successfully.", transaction.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction {TransactionId} failed.", transaction.Id);
            transaction.MarkFailed();
            await transactionRepository.UpdateAsync(transaction, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }

    private async Task<Transaction> CreateConversionAsync(ApplyTransactionCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.ConvertedCurrencyCode))
            throw new ArgumentException("Target currency is required", nameof(command.ConvertedCurrencyCode));

        var sourceRates = await queryDispatcher.QueryAsync<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>(
            new GetExchangeRatesQuery(null, command.CurrencyCode), cancellationToken);
        var targetRates = await queryDispatcher.QueryAsync<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>(
            new GetExchangeRatesQuery(null, command.ConvertedCurrencyCode), cancellationToken);

        var sourceRate = sourceRates.FirstOrDefault()?.Rate
            ?? throw new InvalidOperationException($"No exchange rate for {command.CurrencyCode}");
        var targetRate = targetRates.FirstOrDefault()?.Rate
            ?? throw new InvalidOperationException($"No exchange rate for {command.ConvertedCurrencyCode}");

        var plnAmount = command.Amount * sourceRate;
        var convertedAmount = Math.Round(plnAmount / targetRate, 4);

        return Transaction.CreateConversion(
            command.WalletId,
            command.CurrencyCode,
            command.Amount,
            command.ConvertedCurrencyCode,
            convertedAmount);
    }
}
