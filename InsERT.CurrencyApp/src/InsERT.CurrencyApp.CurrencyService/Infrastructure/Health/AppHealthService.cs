using InsERT.CurrencyApp.Abstractions.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.Health;

public class AppHealthService(HealthCheckService healthCheckService) : IAppHealthService
{
    private readonly HealthCheckService _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));

    public async Task<AppHealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var report = await _healthCheckService.CheckHealthAsync(cancellationToken);

        return new AppHealthStatus
        {
            Status = report.Status.ToString(),
            Duration = report.TotalDuration,
            Checks = [.. report.Entries.Select(entry => new AppHealthCheckResult
            {
                Name = entry.Key,
                Status = entry.Value.Status.ToString(),
                Description = entry.Value.Description ?? string.Empty
            })]
        };
    }
}
