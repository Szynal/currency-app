using System.ComponentModel.DataAnnotations;

namespace InsERT.CurrencyApp.TransactionService.WebApi.Models.Requests;

public sealed record CreateWithdrawRequest
{
    [Required]
    public Guid WalletId { get; init; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; init; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string CurrencyCode { get; init; } = string.Empty;
}
