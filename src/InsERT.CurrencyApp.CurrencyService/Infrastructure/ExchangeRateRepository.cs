using InsERT.CurrencyApp.CurrencyService.DataAccess;
using InsERT.CurrencyApp.CurrencyService.Domain;
using InsERT.CurrencyApp.CurrencyService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure;

public class ExchangeRateRepository(CurrencyDbContext db) : IExchangeRateRepository
{
    private readonly CurrencyDbContext _db = db;

    public async Task<HashSet<string>> GetExistingCodesAsync(DateOnly date, IEnumerable<string> incomingCodes, CancellationToken cancellationToken)
    {
        var codes = await _db.ExchangeRates
            .Where(e => e.EffectiveDate == date && incomingCodes.Contains(e.Code))
            .Select(e => e.Code)
            .ToListAsync(cancellationToken);

        return [.. codes];
    }

    public Task AddManyAsync(IEnumerable<ExchangeRate> rates, CancellationToken cancellationToken)
    {
        _db.ExchangeRates.AddRange(rates);
        return Task.CompletedTask;
    }

    public Task SaveAsync(CancellationToken cancellationToken) => _db.SaveChangesAsync(cancellationToken);
}
