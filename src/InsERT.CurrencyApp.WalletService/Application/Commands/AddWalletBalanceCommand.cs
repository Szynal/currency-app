using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;

namespace InsERT.CurrencyApp.WalletService.Application.Commands;

public sealed record AddWalletBalanceCommand(
    Guid WalletId,
    string CurrencyCode,
    decimal InitialAmount
) : ICommand<Unit>;
