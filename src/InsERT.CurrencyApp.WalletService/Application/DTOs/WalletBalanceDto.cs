namespace InsERT.CurrencyApp.WalletService.Application.DTOs;

public class WalletBalanceDto
{
    public required string CurrencyCode { get; set; }
    public decimal Amount { get; set; }
}
