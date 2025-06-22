using InsERT.CurrencyApp.CurrencyService.Configuration;
using InsERT.CurrencyApp.CurrencyService.DataAccess;
using InsERT.CurrencyApp.CurrencyService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.DI;

public static class DataAccessModule
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddDbContext<CurrencyDbContext>((sp, options) =>
        {
            var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
            options.UseNpgsql(settings.ConnectionString);
        });

        services.AddHealthChecks().AddNpgSql(
            connectionStringFactory: sp => sp.GetRequiredService<IOptions<AppSettings>>().Value.ConnectionString,
            name: "Postgres",
            failureStatus: HealthStatus.Unhealthy,
            tags: ["db", "postgres"]);

        services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();

        return services;
    }
}
