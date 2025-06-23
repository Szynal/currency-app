namespace InsERT.CurrencyApp.Abstractions.Events;

public record TransactionCreatedEvent(Guid TransactionId, Guid WalletId, decimal Amount, string CurrencyCode);
