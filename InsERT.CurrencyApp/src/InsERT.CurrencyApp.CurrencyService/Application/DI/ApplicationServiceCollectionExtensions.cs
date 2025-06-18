using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.CurrencyService.Application.CQRS;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Commands;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Handlers;
using InsERT.CurrencyApp.CurrencyService.Configuration;
using System.Reflection;

namespace InsERT.CurrencyApp.CurrencyService.Application.DI
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            AppSettings settings,
            IReadOnlyCollection<Assembly> applicationAssemblies)
        {
            services.AddScoped<IDispatcher, DefaultDispatcher>();
            services.AddScoped<ICommandHandler<StoreExchangeRatesCommand, int>, StoreExchangeRatesHandler>();

            return services;
        }
    }
}
