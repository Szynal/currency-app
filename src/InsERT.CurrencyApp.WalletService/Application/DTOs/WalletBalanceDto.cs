namespace InsERT.CurrencyApp.WalletService.Application.DTOs;

public sealed record WalletBalanceDto
{
    public string CurrencyCode { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
