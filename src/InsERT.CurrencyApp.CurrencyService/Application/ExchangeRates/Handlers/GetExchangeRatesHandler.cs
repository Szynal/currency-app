using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.Abstractions.Currency.Models;
using InsERT.CurrencyApp.Abstractions.Currency.Queries;
using InsERT.CurrencyApp.CurrencyService.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Handlers;

public sealed class GetExchangeRatesHandler(CurrencyDbContext dbContext) : IQueryHandler<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>
{
    private readonly CurrencyDbContext _dbContext = dbContext;

    public async Task<IEnumerable<ExchangeRateDto>> HandleAsync(GetExchangeRatesQuery query, CancellationToken cancellationToken = default)
    {
        var exchangeRates = _dbContext.ExchangeRates.AsQueryable();

        if (query.Date is not null)
        {
            exchangeRates = exchangeRates.Where(r => r.EffectiveDate == query.Date.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            var normalizedCode = query.Code.ToLower();
            exchangeRates = exchangeRates.Where(r => r.Code.ToLower() == normalizedCode);
        }

        return await exchangeRates
            .OrderByDescending(r => r.EffectiveDate)
            .Select(r => new ExchangeRateDto
            {
                Currency = r.Currency,
                Code = r.Code,
                Rate = r.Rate,
                EffectiveDate = r.EffectiveDate
            })
            .ToListAsync(cancellationToken);
    }
}
