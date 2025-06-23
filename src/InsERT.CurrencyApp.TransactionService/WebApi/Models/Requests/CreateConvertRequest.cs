using System;
using System.ComponentModel.DataAnnotations;

namespace InsERT.CurrencyApp.TransactionService.WebApi.Models.Requests;

public record CreateConvertRequest(
    [property: Required] Guid WalletId,
    [property: Range(0.01, double.MaxValue)] decimal SourceAmount,
    [property: Required, StringLength(3, MinimumLength = 3)] string SourceCurrencyCode,
    [property: Range(0.01, double.MaxValue)] decimal TargetAmount,
    [property: Required, StringLength(3, MinimumLength = 3)] string TargetCurrencyCode);
