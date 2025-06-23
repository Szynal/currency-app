using Microsoft.EntityFrameworkCore;
using InsERT.CurrencyApp.TransactionService.Domain.Entities;
using InsERT.CurrencyApp.TransactionService.Domain.Repositories;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.DataAccess;

public class TransactionRepository(TransactionDbContext dbContext) : ITransactionRepository
{
    private readonly TransactionDbContext _dbContext = dbContext;

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Transaction>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Transactions
            .Where(t => t.WalletId == walletId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _dbContext.Transactions.Update(transaction);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Transaction>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Transactions
            .Where(t => t.Status == TransactionStatus.Pending)
            .ToListAsync(cancellationToken);
    }

}
