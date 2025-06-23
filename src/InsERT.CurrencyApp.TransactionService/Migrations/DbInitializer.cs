using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using Npgsql;

namespace InsERT.CurrencyApp.TransactionService.Migrations;

public static class DbInitializer
{
    public static async Task EnsureDatabaseMigratedAsync<TDbContext>(
        IServiceProvider services,
        ILogger logger,
        CancellationToken cancellationToken = default)
        where TDbContext : DbContext
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        AsyncRetryPolicy retryPolicy = Policy
            .Handle<PostgresException>(ex => ex.SqlState == "57P03") // Database is in recovery mode
            .WaitAndRetryAsync(
                retryCount: 10,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                onRetry: (exception, timeSpan, attempt, _) =>
                {
                    logger.LogWarning(
                        "Retry {Attempt}: Database not ready (waiting {Delay}). Reason: {Message}",
                        attempt, timeSpan, exception.Message);
                });

        await retryPolicy.ExecuteAsync(async ct =>
        {
            logger.LogInformation("Applying EF Core migrations...");
            await dbContext.Database.MigrateAsync(ct);
            logger.LogInformation("Migrations applied successfully.");
        }, cancellationToken);
    }
}
