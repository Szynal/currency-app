namespace InsERT.CurrencyApp.WalletService.Domain.Entities;

public class Wallet
{
    private readonly List<WalletBalance> _balances = [];

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public IReadOnlyCollection<WalletBalance> Balances => _balances.AsReadOnly();

    private Wallet() { }

    public static Wallet Create(Guid userId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Wallet name cannot be empty.", nameof(name));

        return new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name.Trim()
        };
    }

    public void ApplyDeposit(string currencyCode, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code is required.", nameof(currencyCode));

        if (amount <= 0)
            throw new InvalidOperationException("Deposit amount must be positive.");

        var balance = _balances.FirstOrDefault(b => b.CurrencyCode == currencyCode.ToUpperInvariant());

        if (balance is not null)
        {
            balance.Increase(amount);
        }
        else
        {
            _balances.Add(new WalletBalance(Id, currencyCode.ToUpperInvariant(), amount));
        }
    }

    public void ApplyWithdrawal(string currencyCode, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code is required.", nameof(currencyCode));

        if (amount <= 0)
            throw new InvalidOperationException("Withdrawal amount must be positive.");

        var balance = _balances.FirstOrDefault(b => b.CurrencyCode == currencyCode.ToUpperInvariant())
                      ?? throw new InvalidOperationException($"Balance for currency '{currencyCode}' does not exist.");

        balance.Decrease(amount);
    }

    public void ApplyConversion(string sourceCurrency, decimal sourceAmount, string targetCurrency, decimal targetAmount)
    {
        if (sourceCurrency.Equals(targetCurrency, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Source and target currencies must be different for conversion.");

        ApplyWithdrawal(sourceCurrency, sourceAmount);
        ApplyDeposit(targetCurrency, targetAmount);
    }
}
