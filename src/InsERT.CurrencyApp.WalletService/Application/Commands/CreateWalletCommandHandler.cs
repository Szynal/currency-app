using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.WalletService.Domain.Entities;
using InsERT.CurrencyApp.WalletService.Domain.Repositories;

namespace InsERT.CurrencyApp.WalletService.Application.Commands;

public sealed class CreateWalletCommandHandler : ICommandHandler<CreateWalletCommand, Guid>
{
    private readonly IWalletRepository _walletRepository;

    public CreateWalletCommandHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<Guid> HandleAsync(CreateWalletCommand command, CancellationToken cancellationToken = default)
    {
        var existingWallets = await _walletRepository.GetByUserIdAsync(command.UserId, cancellationToken);

        var walletAlreadyExists = existingWallets
            .Any(w => string.Equals(w.Name, command.Name, StringComparison.OrdinalIgnoreCase));

        if (walletAlreadyExists)
        {
            throw new InvalidOperationException(
                $"A wallet named '{command.Name}' already exists for user '{command.UserId}'.");
        }

        var wallet = Wallet.Create(command.UserId, command.Name, new[] { ("MGA", 100.0m), ("PLN", 3m) });

        await _walletRepository.AddAsync(wallet, cancellationToken);
        await _walletRepository.SaveChangesAsync(cancellationToken);

        return wallet.Id;
    }
}
