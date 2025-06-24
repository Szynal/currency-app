using InsERT.CurrencyApp.TransactionService.Domain.Entities;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;

public interface IWalletServiceClient
{
    Task ApplyTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
}
