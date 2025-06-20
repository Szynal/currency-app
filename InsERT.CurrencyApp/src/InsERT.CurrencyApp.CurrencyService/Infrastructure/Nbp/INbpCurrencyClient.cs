namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

public interface INbpCurrencyClient
{
    Task<NbpTable?> FetchLatestRatesTableAsync(CancellationToken cancellationToken);
}
