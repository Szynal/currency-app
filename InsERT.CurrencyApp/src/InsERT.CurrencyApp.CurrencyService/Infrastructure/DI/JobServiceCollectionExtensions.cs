using InsERT.CurrencyApp.CurrencyService.Configuration;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.DI;

public static class JobServiceCollectionExtensions
{
    public static IServiceCollection AddJobScheduling(
        this IServiceCollection services,
        AppSettings settings)
    {
        services.AddHostedService<CurrencyRateFetcher>();

        // posiible Hangfire / Quartz in future ... 
        // services.AddHangfire(...)
        // services.AddQuartz(...)

        return services;
    }
}
