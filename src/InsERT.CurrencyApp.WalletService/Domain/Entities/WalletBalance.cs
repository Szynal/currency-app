namespace InsERT.CurrencyApp.WalletService.Domain.Entities;

public sealed class WalletBalance
{
    public Guid Id { get; private set; }
    public Guid WalletId { get; private set; }
    public string CurrencyCode { get; private set; } = null!;
    public decimal Amount { get; private set; }

    public WalletBalance(Guid walletId, string currencyCode, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code is required.", nameof(currencyCode));

        if (currencyCode.Length != 3)
            throw new ArgumentException("Currency code must be 3 characters long.", nameof(currencyCode));

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
            throw new InvalidOperationException("Increase amount must be greater than zero.");

        Amount += amount;
    }

    public void Decrease(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Decrease amount must be greater than zero.");

        if (amount > Amount)
            throw new InvalidOperationException($"Cannot withdraw {amount}. Available: {Amount}");

        Amount -= amount;
    }

    private WalletBalance() { }
}
