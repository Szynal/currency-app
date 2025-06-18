using InsERT.CurrencyApp.CurrencyService.Configuration;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        AppSettings settings)
    {
        services.AddDbContext<CurrencyDbContext>(options =>
            options.UseNpgsql(settings.ConnectionString));

        services.AddHealthChecks().AddNpgSql(settings.ConnectionString);
        services.AddHttpClient();

        services.AddScoped<INbpCurrencyClient, NbpCurrencyClient>();

        return services;
    }
}
