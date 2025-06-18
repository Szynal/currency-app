using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Commands;
using InsERT.CurrencyApp.CurrencyService.Configuration;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure;

public class CurrencyRateFetcher(
    ILogger<CurrencyRateFetcher> logger,
    IServiceProvider provider,
    AppSettings settings) : BackgroundService
{
    private readonly ILogger<CurrencyRateFetcher> _logger = logger;
    private readonly IServiceProvider _provider = provider;
    private readonly TimeSpan _fetchInterval = TimeSpan.FromMinutes(settings.FetchIntervalMinutes);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("CurrencyRateFetcher started. Interval: {interval} minutes", _fetchInterval.TotalMinutes);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("CurrencyRateFetcher tick at {time}", DateTime.Now.ToString("HH:mm:ss"));

                await using var scope = _provider.CreateAsyncScope();
                var nbpClient = scope.ServiceProvider.GetRequiredService<INbpCurrencyClient>();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

                var table = await nbpClient.GetLatestTableAsync(cancellationToken);

                if (table is not null)
                {
                    var added = await dispatcher.SendAsync<StoreExchangeRatesCommand, int>(
                        new StoreExchangeRatesCommand(table), cancellationToken);

                    _logger.LogInformation("Saved {count} exchange rates for {date}", added, table.EffectiveDate);
                }
                else
                {
                    _logger.LogWarning("NBP returned no data.");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyRateFetcher failed");
            }

            await Task.Delay(_fetchInterval, cancellationToken);
        }
    }
}
