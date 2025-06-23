using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.Abstractions.Currency.Models;
using InsERT.CurrencyApp.Abstractions.Currency.Queries;
using InsERT.CurrencyApp.TransactionService.Domain;
using InsERT.CurrencyApp.TransactionService.Domain.Entities;
using InsERT.CurrencyApp.TransactionService.Domain.Repositories;
using InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;

namespace InsERT.CurrencyApp.TransactionService.Application.Commands.Handlers
{
    public class ApplyTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IWalletServiceClient walletServiceClient,
        IUnitOfWork unitOfWork,
        IQueryDispatcher queryDispatcher,
        ILogger<ApplyTransactionCommandHandler> logger) : ICommandHandler<ApplyTransactionCommand, Unit>
    {
        private readonly ITransactionRepository _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        private readonly IWalletServiceClient _walletServiceClient = walletServiceClient ?? throw new ArgumentNullException(nameof(walletServiceClient));
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly IQueryDispatcher _queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        private readonly ILogger<ApplyTransactionCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<Unit> HandleAsync(ApplyTransactionCommand command, CancellationToken cancellationToken = default)
        {

            Transaction transaction = command.Type switch
            {
                TransactionType.Deposit => Transaction.CreateDeposit(command.WalletId, command.CurrencyCode, command.Amount),
                TransactionType.Withdraw => Transaction.CreateWithdrawal(command.WalletId, command.CurrencyCode, command.Amount),
                TransactionType.ConvertCurrency => Transaction.CreateConversion(
                    command.WalletId,
                    command.CurrencyCode,
                    command.Amount,
                    command.ConvertedCurrencyCode ?? throw new ArgumentNullException(nameof(command.ConvertedCurrencyCode)),
                    command.ConvertedAmount ?? throw new ArgumentNullException(nameof(command.ConvertedAmount))),
                _ => throw new InvalidOperationException("Invalid transaction type")
            };

            if (transaction.Type == TransactionType.ConvertCurrency)
            {
                var calculatedAmount = await CalculateConvertedAmountAsync(
                    transaction.CurrencyCode,
                    transaction.Amount,
                    transaction.ConvertedCurrencyCode!,
                    cancellationToken);

                if (Math.Abs(calculatedAmount - transaction.ConvertedAmount.GetValueOrDefault()) > 0.0001m)
                    throw new InvalidOperationException("Converted amount does not match the current exchange rates.");
            }

            try
            {
                _logger.LogInformation("Processing new transaction of type {TransactionType} for wallet {WalletId}.", transaction.Type, transaction.WalletId);

                await _transactionRepository.AddAsync(transaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _walletServiceClient.ApplyTransactionAsync(transaction, cancellationToken);

                transaction.MarkAccepted();
                await _transactionRepository.UpdateAsync(transaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Transaction {TransactionId} accepted and applied to wallet.", transaction.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply transaction for wallet {WalletId}. Marking as failed.", transaction.WalletId);

                transaction.MarkFailed();
                await _transactionRepository.UpdateAsync(transaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

            }

            return Unit.Value;
        }

        private async Task<decimal> CalculateConvertedAmountAsync(
            string sourceCurrency,
            decimal sourceAmount,
            string targetCurrency,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sourceCurrency))
                throw new ArgumentException("Source currency must be provided.", nameof(sourceCurrency));
            if (string.IsNullOrWhiteSpace(targetCurrency))
                throw new ArgumentException("Target currency must be provided.", nameof(targetCurrency));
            if (sourceAmount <= 0)
                throw new ArgumentOutOfRangeException(nameof(sourceAmount), "Amount must be positive.");

            if (sourceCurrency.Equals(targetCurrency, StringComparison.OrdinalIgnoreCase))
                return sourceAmount;

            var sourceRates = await _queryDispatcher.QueryAsync<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>(
                new GetExchangeRatesQuery(null, sourceCurrency), cancellationToken);

            var targetRates = await _queryDispatcher.QueryAsync<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>(
                new GetExchangeRatesQuery(null, targetCurrency), cancellationToken);

            var sourceRate = sourceRates.FirstOrDefault()?.Rate
                ?? throw new InvalidOperationException($"No exchange rate found for currency '{sourceCurrency}'.");

            var targetRate = targetRates.FirstOrDefault()?.Rate
                ?? throw new InvalidOperationException($"No exchange rate found for currency '{targetCurrency}'.");

            var amountInPln = sourceAmount * sourceRate;
            var convertedAmount = amountInPln / targetRate;

            return Math.Round(convertedAmount, 4);
        }
    }
}
