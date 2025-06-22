using InsERT.CurrencyApp.CurrencyService.DataAccess;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Polly;
using Polly.Retry;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure;

public static class DbInitializer
{
    public static async Task EnsureDatabaseMigratedAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();

        AsyncRetryPolicy retryPolicy = Policy
            .Handle<PostgresException>(ex => ex.SqlState == "57P03") // DB still starting
            .WaitAndRetryAsync(
                retryCount: 10,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                onRetry: (exception, timeSpan, attempt, _) =>
                {
                    logger.LogWarning("Retry {Attempt}: DB not ready, waiting {Delay}. Reason: {Message}", attempt, timeSpan, exception.Message);
                });

        await retryPolicy.ExecuteAsync(async () =>
        {
            logger.LogInformation("Applying database migrations...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database ready.");
        });
    }
}
