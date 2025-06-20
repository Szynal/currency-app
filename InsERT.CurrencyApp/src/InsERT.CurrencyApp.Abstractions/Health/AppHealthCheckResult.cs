namespace InsERT.CurrencyApp.Abstractions.Health;

public class AppHealthCheckResult
{
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string Description { get; set; }
}
