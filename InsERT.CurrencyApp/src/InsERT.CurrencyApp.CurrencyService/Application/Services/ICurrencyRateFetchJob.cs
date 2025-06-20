namespace InsERT.CurrencyApp.CurrencyService.Application.Services;

public interface ICurrencyRateFetchJob
{
    Task FetchAndStoreAsync(CancellationToken cancellationToken = default);
}
