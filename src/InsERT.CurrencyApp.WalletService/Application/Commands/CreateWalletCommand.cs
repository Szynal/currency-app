using InsERT.CurrencyApp.Abstractions.CQRS.Commands;

namespace InsERT.CurrencyApp.WalletService.Application.Commands;

public sealed record CreateWalletCommand(Guid UserId, string Name) : ICommand<Guid>;
