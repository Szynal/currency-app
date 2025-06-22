using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Commands;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

namespace InsERT.CurrencyApp.CurrencyService.Application.Services;

public class CurrencyRateFetchJob(
    INbpCurrencyClient nbpClient,
    ICommandDispatcher dispatcher,
    ILogger<CurrencyRateFetchJob> logger) : ICurrencyRateFetchJob
{
    private readonly INbpCurrencyClient _nbpClient = nbpClient;
    private readonly ICommandDispatcher _dispatcher = dispatcher;
    private readonly ILogger<CurrencyRateFetchJob> _logger = logger;

    public async Task FetchAndStoreAsync(CancellationToken cancellationToken = default)
    {
        var table = await _nbpClient.FetchLatestRatesTableAsync(cancellationToken);
        if (table == null)
        {
            _logger.LogWarning("NBP returned no data.");
            return;
        }

        var count = await _dispatcher.SendAsync<StoreExchangeRatesCommand, int>(
            new StoreExchangeRatesCommand(table), cancellationToken);

        _logger.LogInformation("Saved {Count} exchange rates for {EffectiveDate}", count, table.EffectiveDate);
    }
}
