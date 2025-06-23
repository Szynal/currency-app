using System.ComponentModel.DataAnnotations;

namespace InsERT.CurrencyApp.TransactionService.Configuration;

public class AppSettings
{
    [Required]
    public string TransactionDbConnectionString { get; set; } = default!;

    [Required]
    public string WalletServiceBaseUrl { get; set; } = default!;

    [Required]
    public string CurrencyServiceBaseUrl { get; set; } = default!;
}   
