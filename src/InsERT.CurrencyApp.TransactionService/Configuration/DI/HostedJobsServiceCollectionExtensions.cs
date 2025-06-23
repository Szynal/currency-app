using InsERT.CurrencyApp.TransactionService.Infrastructure.HostedServices;

namespace InsERT.CurrencyApp.TransactionService.Configuration.DI;

public static class HostedJobsServiceCollectionExtensions
{
    public static IServiceCollection AddHostedJobs(this IServiceCollection services)
    {
        services.AddHostedService<TransactionProcessingService>();

        return services;
    }
}
