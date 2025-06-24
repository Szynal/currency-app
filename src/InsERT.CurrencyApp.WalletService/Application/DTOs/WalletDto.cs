namespace InsERT.CurrencyApp.WalletService.Application.DTOs;

public class WalletDto
{
    public Guid WalletId { get; set; }
    public required string WalletName { get; set; }
    public required List<WalletBalanceDto> Balances { get; set; }
}