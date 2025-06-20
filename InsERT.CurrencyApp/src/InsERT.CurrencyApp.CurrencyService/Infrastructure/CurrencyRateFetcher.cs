using InsERT.CurrencyApp.CurrencyService.Application.Services;
using InsERT.CurrencyApp.CurrencyService.Configuration;
using Microsoft.Extensions.Options;

public class CurrencyRateFetcher : BackgroundService
{
    private readonly ILogger<CurrencyRateFetcher> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval;

    public CurrencyRateFetcher(
        ILogger<CurrencyRateFetcher> logger,
        IServiceProvider serviceProvider,
        IOptions<AppSettings> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        if (options?.Value is null)
            throw new ArgumentNullException(nameof(options));

        _interval = TimeSpan.FromMinutes(options.Value.FetchIntervalMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("CurrencyRateFetcher started. Interval: {IntervalMinutes} minutes", _interval.TotalMinutes);

        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("CurrencyRateFetcher tick at {Time}", DateTimeOffset.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var job = scope.ServiceProvider.GetRequiredService<ICurrencyRateFetchJob>();
                await job.FetchAndStoreAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CurrencyRateFetcher failed.");
            }

            try
            {
                await Task.Delay(_interval, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("CurrencyRateFetcher was cancelled.");
                break;
            }
        }
    }
}
