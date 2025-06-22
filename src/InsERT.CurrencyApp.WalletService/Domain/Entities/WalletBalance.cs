namespace InsERT.CurrencyApp.WalletService.Domain.Entities;

public class WalletBalance
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string CurrencyCode { get; private set; }

    public decimal Amount { get; private set; }

    public Guid WalletId { get; private set; }

    private WalletBalance() { }

    public WalletBalance(string currencyCode, decimal amount)
    {
        CurrencyCode = currencyCode.ToUpperInvariant();
        Amount = amount;
    }

    public void AddAmount(decimal amount)
    {
        Amount += amount;
    }

    public bool CanWithdraw(decimal amount) => Amount >= amount;

    public void SubtractAmount(decimal amount)
    {
        if (!CanWithdraw(amount))
            throw new InvalidOperationException("Insufficient funds.");

        Amount -= amount;
    }
}
