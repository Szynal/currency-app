using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Models;

namespace InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Queries;

public sealed record GetExchangeRatesQuery(DateOnly? Date, string? Code)
    : IQuery<IEnumerable<ExchangeRateDto>>;
