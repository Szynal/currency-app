using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.WalletService.Application.DTOs;
using InsERT.CurrencyApp.WalletService.Domain.Repositories;

namespace InsERT.CurrencyApp.WalletService.Application.Queries.Handlers;

public class GetUserWalletsHandler : IQueryHandler<GetUserWalletsQuery, UserWalletsDto>
{
    private readonly IWalletRepository _walletRepository;

    public GetUserWalletsHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<UserWalletsDto> HandleAsync(GetUserWalletsQuery query, CancellationToken cancellationToken)
    {
        {
            var wallets = await _walletRepository.GetByUserIdAsync(query.UserId, cancellationToken);

            return new UserWalletsDto
            {
                UserId = query.UserId,
                Wallets = [.. wallets.Select(w => new WalletDto
            {
                WalletId = w.Id,
                WalletName = w.Name,
                Balances = [.. w.Balances.Select(b => new WalletBalanceDto(b.CurrencyCode, b.Amount))]
            })]
            };
        }
    }
}
