namespace InsERT.CurrencyApp.WalletService.Application.DTOs;

public class UserWalletsDto
{
    public Guid UserId { get; set; }
    public List<WalletDto>? Wallets { get; set; }
}