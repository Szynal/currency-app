namespace InsERT.CurrencyApp.Abstractions.Health;

public interface IAppHealthService
{
    Task<AppHealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default);
}
