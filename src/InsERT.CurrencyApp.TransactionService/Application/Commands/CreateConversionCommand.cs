using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;

namespace InsERT.CurrencyApp.TransactionService.Application.Commands;

public record CreateConversionCommand(
    Guid WalletId,
    string SourceCurrencyCode,
    decimal SourceAmount,
    string TargetCurrencyCode
) : ICommand<Unit>;
