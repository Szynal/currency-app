using InsERT.CurrencyApp.TransactionService.Domain.Entities;

namespace InsERT.CurrencyApp.TransactionService.Domain.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Transaction>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Transaction>> GetPendingTransactionsAsync(int limit, CancellationToken cancellationToken = default);

    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);

    Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
