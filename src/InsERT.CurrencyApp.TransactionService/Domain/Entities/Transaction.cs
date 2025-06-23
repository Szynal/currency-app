namespace InsERT.CurrencyApp.TransactionService.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid WalletId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; } = null!;
    public TransactionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    public decimal? ConvertedAmount { get; private set; }
    public string? ConvertedCurrencyCode { get; private set; }

    private Transaction() { }

    public static Transaction CreateDeposit(Guid walletId, string currencyCode, decimal amount)
    {
        ValidateAmount(amount);

        return new Transaction
        {
            Id = Guid.NewGuid(),
            WalletId = walletId,
            Type = TransactionType.Deposit,
            Amount = amount,
            CurrencyCode = currencyCode.ToUpperInvariant(),
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Transaction CreateWithdrawal(Guid walletId, string currencyCode, decimal amount)
    {
        ValidateAmount(amount);

        return new Transaction
        {
            Id = Guid.NewGuid(),
            WalletId = walletId,
            Type = TransactionType.Withdraw,
            Amount = amount,
            CurrencyCode = currencyCode.ToUpperInvariant(),
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Transaction CreateConversion(
        Guid walletId,
        string sourceCurrency,
        decimal sourceAmount,
        string targetCurrency,
        decimal targetAmount)
    {
        ValidateAmount(sourceAmount);
        ValidateAmount(targetAmount);

        if (sourceCurrency.Equals(targetCurrency, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Source and target currency must differ.");

        return new Transaction
        {
            Id = Guid.NewGuid(),
            WalletId = walletId,
            Type = TransactionType.ConvertCurrency,
            Amount = sourceAmount,
            CurrencyCode = sourceCurrency.ToUpperInvariant(),
            ConvertedAmount = targetAmount,
            ConvertedCurrencyCode = targetCurrency.ToUpperInvariant(),
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.");
    }

    public void MarkAccepted()
    {
        Status = TransactionStatus.Accepted;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkRejected()
    {
        Status = TransactionStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkFailed()
    {
        Status = TransactionStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
    }
}
