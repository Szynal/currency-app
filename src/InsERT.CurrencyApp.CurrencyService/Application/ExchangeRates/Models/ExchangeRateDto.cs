namespace InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Models;

public sealed class ExchangeRateDto
{
    public required string Currency { get; init; }
    public required string Code { get; init; }
    public required decimal Rate { get; init; }
    public required DateOnly EffectiveDate { get; init; }
}
