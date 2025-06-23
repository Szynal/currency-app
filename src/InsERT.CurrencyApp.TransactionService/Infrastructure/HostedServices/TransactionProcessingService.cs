using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.TransactionService.Application.Commands;
using InsERT.CurrencyApp.TransactionService.Domain;
using InsERT.CurrencyApp.TransactionService.Domain.Entities;
using InsERT.CurrencyApp.TransactionService.Domain.Repositories;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.HostedServices;

public class TransactionProcessingService : BackgroundService
{
    private readonly ILogger<TransactionProcessingService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public TransactionProcessingService(
        ILogger<TransactionProcessingService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Transaction processing background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                var commandDispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var pendingTransactions = await transactionRepository.GetPendingTransactionsAsync(stoppingToken);

                foreach (var transaction in pendingTransactions)
                {
                    _logger.LogInformation("Processing transaction {TransactionId} of type {TransactionType}", transaction.Id, transaction.Type);

                    try
                    {
                        var command = CreateApplyTransactionCommand(transaction);

                        await commandDispatcher.SendAsync<ApplyTransactionCommand, Unit>(command, stoppingToken);

                        transaction.MarkAccepted();
                        await transactionRepository.UpdateAsync(transaction, stoppingToken);

                        _logger.LogInformation("Transaction {TransactionId} accepted.", transaction.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing transaction {TransactionId}", transaction.Id);

                        transaction.MarkRejected();
                        await transactionRepository.UpdateAsync(transaction, stoppingToken);
                    }
                }

                await unitOfWork.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in transaction processing loop.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
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
