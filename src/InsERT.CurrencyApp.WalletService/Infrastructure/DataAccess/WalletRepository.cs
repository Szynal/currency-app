using InsERT.CurrencyApp.WalletService.Domain.Entities;
using InsERT.CurrencyApp.WalletService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.DataAccess;

public sealed class WalletRepository : IWalletRepository
{
    private readonly WalletDbContext _dbContext;

    public WalletRepository(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Wallet?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Wallets
            .Include(w => w.Balances)
            .FirstOrDefaultAsync(w => w.Id == walletId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Wallet>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Wallets
            .Include(w => w.Balances)
            .Where(w => w.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Wallet wallet, CancellationToken cancellationToken = default)
    {
        await _dbContext.Wallets.AddAsync(wallet, cancellationToken);
    }

    public void AddBalance(WalletBalance balance)
    {
        _dbContext.WalletBalances.Add(balance);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Wallet>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.Wallets
            .Include(w => w.Balances)
            .Where(w => w.UserId == userId)
            .ToListAsync();
    }

}
