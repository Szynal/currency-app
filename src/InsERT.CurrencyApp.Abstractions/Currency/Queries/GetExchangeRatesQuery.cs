using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.Abstractions.Currency.Models;

namespace InsERT.CurrencyApp.Abstractions.Currency.Queries
{
    public sealed record GetExchangeRatesQuery(DateOnly? Date, string? Code)
        : IQuery<IEnumerable<ExchangeRateDto>>;
}
