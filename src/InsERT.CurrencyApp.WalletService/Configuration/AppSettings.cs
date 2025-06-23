using System.ComponentModel.DataAnnotations;

namespace InsERT.CurrencyApp.WalletService.Configuration;

public sealed class AppSettings
{
    [Required]
    public string WalletDbConnectionString { get; init; } = string.Empty;
}
