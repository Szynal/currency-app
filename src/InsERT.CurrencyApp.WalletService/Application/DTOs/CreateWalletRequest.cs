namespace InsERT.CurrencyApp.WalletService.Application.DTOs;

public sealed record CreateWalletRequest
{
    public string Name { get; init; } = string.Empty;
}
