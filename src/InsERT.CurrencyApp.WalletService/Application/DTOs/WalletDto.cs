namespace InsERT.CurrencyApp.WalletService.Application.DTOs;

public class WalletDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<WalletBalanceDto> Balances { get; set; } = new();
}
