using InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.DI;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddHostedJobs(this IServiceCollection services)
    {
        services.AddScoped<INbpCurrencyClient, NbpCurrencyClient>();
        return services;
    }
}
