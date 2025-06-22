using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using InsERT.CurrencyApp.WalletService.Configuration;
using InsERT.CurrencyApp.WalletService.DataAccess;
using InsERT.CurrencyApp.WalletService.Domain.Repositories;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.DI
{
    public static class DataAccessModule
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            services.AddDbContext<WalletDbContext>((sp, options) =>
            {
                var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
                options.UseNpgsql(settings.ConnectionString);
            });

            services.AddHealthChecks()
                .AddNpgSql(
                    connectionStringFactory: sp => sp.GetRequiredService<IOptions<AppSettings>>().Value.ConnectionString,
                    name: "Postgres",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "postgres" }
                );

            services.AddScoped<IWalletRepository, WalletRepository>();

            return services;
        }
    }
}
