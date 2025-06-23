using System.ComponentModel.DataAnnotations;

namespace InsERT.CurrencyApp.WalletService.WebApi.Models.Requests;

public sealed class ApplyTransactionRequest
{
    [Required]
    public Guid WalletId { get; init; }

    [Required]
    public Guid TransactionId { get; init; }

    [Required]
    public string Type { get; init; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; init; }

    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 letters.")]
    public string CurrencyCode { get; init; } = string.Empty;

    public decimal? ConvertedAmount { get; init; }

    [StringLength(3, MinimumLength = 3, ErrorMessage = "Converted currency code must be exactly 3 letters.")]
    public string? ConvertedCurrencyCode { get; init; }
}
