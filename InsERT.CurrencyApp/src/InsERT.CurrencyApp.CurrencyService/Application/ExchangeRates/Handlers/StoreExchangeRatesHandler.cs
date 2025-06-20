using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Commands;
using InsERT.CurrencyApp.CurrencyService.DataAccess;
using InsERT.CurrencyApp.CurrencyService.Domain;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Handlers;

public class StoreExchangeRatesHandler(CurrencyDbContext db) : ICommandHandler<StoreExchangeRatesCommand, int>
{
    private readonly CurrencyDbContext _db = db;

    public async Task<int> HandleAsync(StoreExchangeRatesCommand command, CancellationToken cancellationToken = default)
    {
        if (command?.Table?.Rates is null || command.Table.Rates.Count == 0)
            return 0;

        var table = command.Table;
        var effectiveDate = DateOnly.Parse(table.EffectiveDate);
        var incomingCodes = table.Rates.Select(r => r.Code).ToHashSet();

        var existingCodes = await _db.ExchangeRates
            .Where(e => e.EffectiveDate == effectiveDate && incomingCodes.Contains(e.Code))
            .Select(e => e.Code)
            .ToListAsync(cancellationToken);

        var newRates = table.Rates
            .Where(r => !existingCodes.Contains(r.Code))
            .Select(r => new ExchangeRate
            {
                Code = r.Code,
                Currency = r.Currency,
                Rate = r.Mid,
                EffectiveDate = effectiveDate
            })
            .ToList();

        if (newRates.Count != 0)
        {
            _db.ExchangeRates.AddRange(newRates);
            await _db.SaveChangesAsync(cancellationToken);
        }

        return newRates.Count;
    }
}
