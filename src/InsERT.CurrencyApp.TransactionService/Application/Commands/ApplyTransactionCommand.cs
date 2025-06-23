using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.TransactionService.Domain.Entities;

namespace InsERT.CurrencyApp.TransactionService.Application.Commands;

public record ApplyTransactionCommand(
    Guid WalletId,
    TransactionType Type,
    decimal Amount,
    string CurrencyCode,
    decimal? ConvertedAmount = null,
    string? ConvertedCurrencyCode = null
) : ICommand<Unit>;
