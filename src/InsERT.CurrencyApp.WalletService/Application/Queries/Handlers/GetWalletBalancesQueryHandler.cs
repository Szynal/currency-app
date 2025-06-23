using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.WalletService.Application.DTOs;
using InsERT.CurrencyApp.WalletService.Domain.Repositories;

namespace InsERT.CurrencyApp.WalletService.Application.Queries.Handlers;

public sealed class GetWalletBalancesQueryHandler : IQueryHandler<GetWalletBalancesQuery, IReadOnlyCollection<WalletBalanceDto>>
{
    private readonly IWalletRepository _walletRepository;

    public GetWalletBalancesQueryHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<IReadOnlyCollection<WalletBalanceDto>> HandleAsync(
        GetWalletBalancesQuery query,
        CancellationToken cancellationToken = default)
    {
        var wallets = await _walletRepository.GetByUserIdAsync(query.UserId, cancellationToken);

        return wallets
            .SelectMany(wallet => wallet.Balances)
            .Select(balance => new WalletBalanceDto(balance.CurrencyCode, balance.Amount))
            .ToArray();
    }
}
