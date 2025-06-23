namespace InsERT.CurrencyApp.WalletService.WebApi.Models.Requests;

public sealed record AddBalanceRequest(string CurrencyCode, decimal InitialAmount);
