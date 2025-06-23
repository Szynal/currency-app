using InsERT.CurrencyApp.WalletService.Domain.Entities;

namespace InsERT.CurrencyApp.WalletService.Domain.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Wallet>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(Wallet wallet, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    void AddBalance(WalletBalance balance);
}
