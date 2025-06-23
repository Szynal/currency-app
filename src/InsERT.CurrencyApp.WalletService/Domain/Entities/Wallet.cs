namespace InsERT.CurrencyApp.WalletService.Domain.Entities;

public class Wallet
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;

    private readonly List<WalletBalance> _balances = new();
    public IReadOnlyCollection<WalletBalance> Balances => _balances.AsReadOnly();

    private Wallet() { }

    public static Wallet Create(Guid userId, string name, IEnumerable<(string CurrencyCode, decimal Amount)>? initialBalances = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Wallet name cannot be empty.", nameof(name));

        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name.Trim()
        };

        if (initialBalances != null)
        {
            foreach (var (currency, amount) in initialBalances)
            {
                var balance = new WalletBalance(currency, amount);
                wallet.AddBalance(balance);
            }
        }

        return wallet;
    }

    public void ApplyDeposit(string currencyCode, decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Deposit amount must be positive.");

        var currency = currencyCode.ToUpperInvariant();
        var balance = _balances.FirstOrDefault(b => b.CurrencyCode == currency);

        if (balance != null)
        {
            balance.Increase(amount);
        }
        else
        {
            var newBalance = new WalletBalance(currency, amount);
            AddBalance(newBalance);
        }
    }

    public void ApplyWithdrawal(string currencyCode, decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Withdrawal amount must be positive.");

        var currency = currencyCode.ToUpperInvariant();
        var balance = _balances.FirstOrDefault(b => b.CurrencyCode == currency)
                      ?? throw new InvalidOperationException($"Balance for currency '{currency}' does not exist.");

        balance.Decrease(amount);
    }

    public void ApplyConversion(string sourceCurrency, decimal sourceAmount, string targetCurrency, decimal targetAmount)
    {
        if (sourceCurrency.Equals(targetCurrency, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Source and target currencies must differ.");

        ApplyWithdrawal(sourceCurrency, sourceAmount);
        ApplyDeposit(targetCurrency, targetAmount);
    }

    public void AddBalance(WalletBalance balance)
    {
        if (_balances.Any(b => b.CurrencyCode == balance.CurrencyCode))
            throw new InvalidOperationException($"Balance for currency '{balance.CurrencyCode}' already exists.");

        balance.SetWallet(this);
        _balances.Add(balance);
    }
}
