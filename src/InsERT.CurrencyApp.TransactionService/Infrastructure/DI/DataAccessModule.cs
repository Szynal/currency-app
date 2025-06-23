using InsERT.CurrencyApp.TransactionService.Configuration;
using InsERT.CurrencyApp.TransactionService.Domain.Repositories;
using InsERT.CurrencyApp.TransactionService.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.DI;

public static class DataAccessModule
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<TransactionDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
            options.UseNpgsql(settings.TransactionDbConnectionString);
        });

        services.AddHealthChecks().AddNpgSql(
            connectionStringFactory: sp => sp.GetRequiredService<IOptions<AppSettings>>().Value.TransactionDbConnectionString,
            name: "transaction-postgres",
            failureStatus: HealthStatus.Unhealthy,
            tags: ["db", "postgres", "transaction"]);

        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}
