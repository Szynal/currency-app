using InsERT.CurrencyApp.WalletService.Configuration;
using InsERT.CurrencyApp.WalletService.Domain.Repositories;
using InsERT.CurrencyApp.WalletService.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.DI;

public static class DataAccessModule
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<WalletDbContext>((serviceProvider, options) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

            options.UseNpgsql(settings.WalletDbConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                npgsqlOptions.CommandTimeout(10);
            });
        });

        services.AddHealthChecks().AddNpgSql(
            connectionStringFactory: sp =>
                sp.GetRequiredService<IOptions<AppSettings>>().Value.WalletDbConnectionString,
            name: "wallet-db",
            failureStatus: HealthStatus.Unhealthy,
            tags: ["db", "postgres", "wallet"]);

        services.AddScoped<IWalletRepository, WalletRepository>();

        return services;
    }
}
