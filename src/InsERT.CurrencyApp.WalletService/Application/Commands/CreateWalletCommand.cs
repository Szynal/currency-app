using InsERT.CurrencyApp.Abstractions.CQRS.Commands;

namespace InsERT.CurrencyApp.WalletService.Application.Commands;

public sealed record CreateWalletCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = default!;
}
