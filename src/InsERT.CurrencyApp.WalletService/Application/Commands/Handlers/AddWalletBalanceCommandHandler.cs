using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.WalletService.Domain.Entities;
using InsERT.CurrencyApp.WalletService.Domain.Repositories;

namespace InsERT.CurrencyApp.WalletService.Application.Commands;

public sealed class AddWalletBalanceCommandHandler : ICommandHandler<AddWalletBalanceCommand, Unit>
{
    private readonly IWalletRepository _walletRepository;

    public AddWalletBalanceCommandHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<Unit> HandleAsync(AddWalletBalanceCommand command, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByIdAsync(command.WalletId, cancellationToken)
                     ?? throw new InvalidOperationException($"Wallet {command.WalletId} not found.");

        var normalizedCurrency = command.CurrencyCode.ToUpperInvariant();

        if (wallet.Balances.Any(b => b.CurrencyCode == normalizedCurrency))
            throw new InvalidOperationException($"Balance for currency '{normalizedCurrency}' already exists.");

        var balance = new WalletBalance(wallet.Id, normalizedCurrency, command.InitialAmount);

        wallet.AddBalance(balance);
        _walletRepository.AddBalance(balance);

        await _walletRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
