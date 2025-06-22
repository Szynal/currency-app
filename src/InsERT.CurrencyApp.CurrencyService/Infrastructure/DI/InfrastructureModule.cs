using InsERT.CurrencyApp.Abstractions.Health;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.Health;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.DI;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDataAccess();
        services.AddHttpClients();
        services.AddHostedService<CurrencyRateFetcher>();
        services.AddScoped<IAppHealthService, AppHealthService>();

        return services;
    }
}