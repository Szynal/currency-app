using InsERT.CurrencyApp.WalletService.Infrastructure.Logging;
using InsERT.CurrencyApp.WalletService.Infrastructure.Resilience;
using Microsoft.Extensions.DependencyInjection;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.DI
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Dodaj moduły infrastruktury (data, HttpClients, retry)
            services
                .AddDataAccess()
                .AddHttpClients();

            // Dodaj cross-cutting: logging, resilience handlers, dekoratory CQRS
            services.AddSingleton<IHttpPolicyFactory, HttpPolicyFactory>();
            services.AddLogging();
            // Możemy dekorować handlery lub repozytoria - np. idempotency, caching itp.

            return services;
        }
    }
}
