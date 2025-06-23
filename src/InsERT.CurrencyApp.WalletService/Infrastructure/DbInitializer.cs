using Microsoft.EntityFrameworkCore;
using Npgsql;
using Polly;
using Polly.Retry;

namespace InsERT.CurrencyApp.WalletService.Infrastructure;

public static class DbInitializer
{
    public static async Task EnsureDatabaseMigratedAsync<TDbContext>(
        IServiceProvider services,
        ILogger logger)
        where TDbContext : DbContext
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        AsyncRetryPolicy retryPolicy = Policy
            .Handle<PostgresException>(ex => ex.SqlState == "57P03") // DB is starting up
            .Or<TimeoutException>()
            .Or<DbUpdateException>()
            .WaitAndRetryAsync(
                retryCount: 10,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                onRetry: (exception, timeSpan, attempt, _) =>
                {
                    logger.LogWarning(
                        "Retry {Attempt}: waiting {Delay}. Reason: {Message}",
                        attempt, timeSpan, exception.Message);
                });

        try
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                logger.LogInformation("Applying EF Core migrations for {Context}...", typeof(TDbContext).Name);
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully for {Context}.", typeof(TDbContext).Name);
            });
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Database migration failed for {Context}.", typeof(TDbContext).Name);
            throw;
        }
    }
}
