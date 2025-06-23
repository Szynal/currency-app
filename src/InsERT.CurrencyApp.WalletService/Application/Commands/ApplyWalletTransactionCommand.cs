using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;

namespace InsERT.CurrencyApp.WalletService.Application.Commands;

public sealed record ApplyWalletTransactionCommand(
    Guid WalletId,
    Guid TransactionId,
    string Type,
    decimal Amount,
    string CurrencyCode,
    decimal? ConvertedAmount = null,
    string? ConvertedCurrencyCode = null
) : ICommand<Unit>;
