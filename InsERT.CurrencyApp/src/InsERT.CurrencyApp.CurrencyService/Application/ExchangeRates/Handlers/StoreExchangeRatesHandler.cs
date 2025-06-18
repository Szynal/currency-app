using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Commands;
using InsERT.CurrencyApp.CurrencyService.Domain;
using InsERT.CurrencyApp.CurrencyService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Handlers;

public class StoreExchangeRatesHandler(CurrencyDbContext db) : ICommandHandler<StoreExchangeRatesCommand, int>
{
    private readonly CurrencyDbContext _db = db;

    public async Task<int> HandleAsync(StoreExchangeRatesCommand command, CancellationToken cancellationToken = default)
    {
        var table = command.Table;
        var effectiveDate = DateOnly.Parse(table.EffectiveDate);
        int added = 0;

        foreach (var rate in table.Rates)
        {
            bool exists = await _db.ExchangeRates.AnyAsync(e =>
                e.Code == rate.Code && e.EffectiveDate == effectiveDate, cancellationToken);

            if (!exists)
            {
                _db.ExchangeRates.Add(new ExchangeRate
                {
                    Code = rate.Code,
                    Currency = rate.Currency,
                    Rate = rate.Mid,
                    EffectiveDate = effectiveDate
                });
                added++;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return added;
    }
}
