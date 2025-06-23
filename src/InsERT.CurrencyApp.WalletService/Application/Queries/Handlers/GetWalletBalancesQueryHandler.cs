using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.WalletService.Application.DTOs;
using InsERT.CurrencyApp.WalletService.Domain.Repositories;

namespace InsERT.CurrencyApp.WalletService.Application.Queries.Handlers;

public sealed class GetWalletBalancesQueryHandler(IWalletRepository walletRepository)
        : IQueryHandler<GetWalletBalancesQuery, IReadOnlyCollection<WalletBalanceDto>>
{
    private readonly IWalletRepository _walletRepository = walletRepository;

    public async Task<IReadOnlyCollection<WalletBalanceDto>> HandleAsync(
        GetWalletBalancesQuery query,
        CancellationToken cancellationToken = default)
    {
        var wallets = await _walletRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        return MapToDtos(wallets);
    }

    private static IReadOnlyCollection<WalletBalanceDto> MapToDtos(IEnumerable<Domain.Entities.Wallet> wallets)
    {
        return [.. wallets
            .SelectMany(wallet => wallet.Balances)
            .Select(balance => new WalletBalanceDto
            {
                CurrencyCode = balance.CurrencyCode,
                Amount = balance.Amount
            })];
    }
}
