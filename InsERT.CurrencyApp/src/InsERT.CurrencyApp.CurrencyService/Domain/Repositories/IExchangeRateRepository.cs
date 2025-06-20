namespace InsERT.CurrencyApp.CurrencyService.Domain.Repositories;

public interface IExchangeRateRepository
{
    Task<HashSet<string>> GetExistingCodesAsync(DateOnly date, IEnumerable<string> incomingCodes, CancellationToken ct);
    Task AddManyAsync(IEnumerable<ExchangeRate> rates, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
