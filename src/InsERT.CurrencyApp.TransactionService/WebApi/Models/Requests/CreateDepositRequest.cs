using System;
using System.ComponentModel.DataAnnotations;

namespace InsERT.CurrencyApp.TransactionService.WebApi.Models.Requests
{
    public class CreateDepositRequest
    {
        [Required]
        public Guid WalletId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
