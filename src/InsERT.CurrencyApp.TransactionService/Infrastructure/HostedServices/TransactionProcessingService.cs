using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.TransactionService.Application.Commands;
using InsERT.CurrencyApp.TransactionService.Domain;
using InsERT.CurrencyApp.TransactionService.Domain.Entities;
using InsERT.CurrencyApp.TransactionService.Domain.Repositories;
using Polly;
using Polly.Retry;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.HostedServices;

public class TransactionProcessingService : BackgroundService
{
    private readonly ILogger<TransactionProcessingService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AsyncRetryPolicy _retryPolicy;

    public TransactionProcessingService(
        ILogger<TransactionProcessingService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}s due to: {Message}", retryCount, timeSpan.TotalSeconds, exception.Message);
                });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Transaction processing background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                    var commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var pendingTransactions = await transactionRepository
                        .GetPendingTransactionsAsync(limit: 100, cancellationToken: stoppingToken);

                    if (pendingTransactions.Count == 0)
                    {
                        _logger.LogDebug("No pending transactions found.");
                        return;
                    }

                    _logger.LogInformation("Found {Count} pending transactions.", pendingTransactions.Count);

                    foreach (var transaction in pendingTransactions)
                    {
                        using (_logger.BeginScope("TransactionId: {TransactionId}", transaction.Id))
                        {
                            try
                            {
                                var command = CreateApplyTransactionCommand(transaction);
                                await commandDispatcher.SendAsync<ApplyTransactionCommand, Unit>(command, stoppingToken);
                                transaction.MarkAccepted();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing transaction.");
                                transaction.MarkRejected();
                            }

                            await transactionRepository.UpdateAsync(transaction, stoppingToken);
                        }
                    }

                    await unitOfWork.SaveChangesAsync(stoppingToken);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in transaction processing loop.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        _logger.LogInformation("Transaction processing background service stopped.");
    }

    private static ApplyTransactionCommand CreateApplyTransactionCommand(Transaction transaction)
    {
        return new ApplyTransactionCommand(
            WalletId: transaction.WalletId,
            Type: transaction.Type,
            Amount: transaction.Amount,
            CurrencyCode: transaction.CurrencyCode,
            ConvertedAmount: transaction.ConvertedAmount,
            ConvertedCurrencyCode: transaction.ConvertedCurrencyCode);
    }
}
