using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.Health;

public class AppHealthService(HealthCheckService healthCheckService) : IAppHealthService
{
    private readonly HealthCheckService _healthCheckService = healthCheckService;

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        var report = await _healthCheckService.CheckHealthAsync(cancellationToken);
        return report.Status == HealthStatus.Healthy;
    }
}
