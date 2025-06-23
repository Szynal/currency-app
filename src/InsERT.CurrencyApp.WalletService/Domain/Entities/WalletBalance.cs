namespace InsERT.CurrencyApp.WalletService.Domain.Entities;

public class WalletBalance
{
    public Guid Id { get; private set; }
    public Guid WalletId { get; private set; }
    public string CurrencyCode { get; private set; } = null!;
    public decimal Amount { get; private set; }

    public Wallet Wallet { get; private set; } = null!;

    private WalletBalance() { } // EF Core

    public WalletBalance(string currencyCode, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code is required.");
        if (currencyCode.Length != 3)
            throw new ArgumentException("Currency code must be 3 characters.");
        if (amount < 0)
            throw new ArgumentException("Amount must be non-negative.");

        Id = Guid.NewGuid();
        CurrencyCode = currencyCode.ToUpperInvariant();
        Amount = amount;
    }
    public WalletBalance(Guid walletId, string currencyCode, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code is required.", nameof(currencyCode));

        if (currencyCode.Length != 3)
            throw new ArgumentException("Currency code must be exactly 3 characters long.", nameof(currencyCode));

        if (amount < 0)
            throw new ArgumentException("Initial amount must be non-negative.", nameof(amount));

        Id = Guid.NewGuid();
        WalletId = walletId;
        CurrencyCode = currencyCode.ToUpperInvariant();
        Amount = amount;
    }

    public void Increase(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Increase amount must be positive.");

        Amount += amount;
    }

    public void Decrease(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Decrease amount must be positive.");
        if (amount > Amount)
            throw new InvalidOperationException($"Insufficient funds. Available: {Amount}, requested: {amount}.");

        Amount -= amount;
    }

    public void SetWallet(Wallet wallet)
    {
        Wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
        WalletId = wallet.Id;
    }
}
