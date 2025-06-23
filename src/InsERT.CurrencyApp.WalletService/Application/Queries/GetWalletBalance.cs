using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.WalletService.Application.DTOs;

namespace InsERT.CurrencyApp.WalletService.Application.Queries;

public sealed record GetWalletBalancesByWalletIdQuery(Guid WalletId)
    : IQuery<IReadOnlyCollection<WalletBalanceDto>>;
