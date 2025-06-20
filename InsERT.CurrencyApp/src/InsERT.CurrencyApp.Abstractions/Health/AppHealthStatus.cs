
namespace InsERT.CurrencyApp.Abstractions.Health;

public class AppHealthStatus
{
    public required string Status { get; set; }
    public TimeSpan Duration { get; set; }
    public required List<AppHealthCheckResult> Checks { get; set; }
}
