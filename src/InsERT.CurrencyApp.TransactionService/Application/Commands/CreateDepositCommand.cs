using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;

namespace InsERT.CurrencyApp.TransactionService.Application.Commands;

public record CreateDepositCommand(
    Guid WalletId,
    string CurrencyCode,
    decimal Amount
) : ICommand<Unit>;
