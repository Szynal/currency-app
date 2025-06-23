using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.WalletService.Application.Commands;
using InsERT.CurrencyApp.WalletService.Domain.Entities;
using InsERT.CurrencyApp.WalletService.Domain.Repositories;

namespace InsERT.CurrencyApp.WalletService.Application.Handlers;

public class ApplyWalletTransactionCommandHandler : ICommandHandler<ApplyWalletTransactionCommand, Unit>
{
    private readonly IWalletRepository _walletRepository;

    public ApplyWalletTransactionCommandHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<Unit> HandleAsync(ApplyWalletTransactionCommand command, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByIdAsync(command.WalletId, cancellationToken)
            ?? throw new InvalidOperationException($"Wallet '{command.WalletId}' not found.");

        EnsureBalanceExists(wallet, command.CurrencyCode);

        switch (command.Type)
        {
            case "Deposit":
                wallet.ApplyDeposit(command.CurrencyCode, command.Amount);
                break;

            case "Withdraw":
                wallet.ApplyWithdrawal(command.CurrencyCode, command.Amount);
                break;

            case "ConvertCurrency":
                if (string.IsNullOrWhiteSpace(command.ConvertedCurrencyCode) || !command.ConvertedAmount.HasValue)
                    throw new InvalidOperationException("Converted currency and amount must be provided for conversion.");

                EnsureBalanceExists(wallet, command.ConvertedCurrencyCode);

                wallet.ApplyConversion(
                    sourceCurrency: command.CurrencyCode,
                    sourceAmount: command.Amount,
                    targetCurrency: command.ConvertedCurrencyCode,
                    targetAmount: command.ConvertedAmount.Value
                );
                break;

            default:
                throw new InvalidOperationException($"Unsupported transaction type: {command.Type}");
        }

        await _walletRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private void EnsureBalanceExists(Wallet wallet, string currencyCode)
    {
        var normalizedCurrency = currencyCode.ToUpperInvariant();
        var balance = wallet.Balances.FirstOrDefault(b => b.CurrencyCode == normalizedCurrency);

        if (balance is null)
        {
            var newBalance = new WalletBalance(wallet.Id, normalizedCurrency, 0m);
            wallet.AddBalance(newBalance);
            _walletRepository.AddBalance(newBalance);
        }
    }
}
