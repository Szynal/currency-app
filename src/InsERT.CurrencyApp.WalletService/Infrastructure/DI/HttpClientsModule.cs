using InsERT.CurrencyApp.Abstractions.Http;
using InsERT.CurrencyApp.WalletService.Configuration;
using InsERT.CurrencyApp.WalletService.Infrastructure.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.DI
{
    public static class HttpClientsModule
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            // Rejestracja klienta do TransactionService
            services
                .AddConfiguredHttpClient<TransactionServiceClientSettings>("TransactionService", settingsSection: "TransactionServiceClient")
                .AddScoped<ITransactionServiceClient, TransactionServiceClient>();

            return services;
        }

        private static IServiceCollection AddConfiguredHttpClient<TSettings>(
            this IServiceCollection services,
            string clientName,
            string settingsSection)
            where TSettings : class, IRetryPolicySettings, new()
        {
            services.AddOptions<TSettings>()
                .BindConfiguration(settingsSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHttpClient(clientName, (sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<TSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                // opcjonalnie: timeout, default headers
            })
            .ConfigurePrimaryHttpMessageHandler(_ =>
                new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.GZip })
            .AddPolicyHandler((sp, request) =>
            {
                var settings = sp.GetRequiredService<IOptions<TSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<TSettings>>();

                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        settings.RetryCount,
                        retry => TimeSpan.FromSeconds(Math.Pow(settings.BackoffSeconds, retry)),
                        (outcome, timespan, retryAttempt, context) =>
                            logger.LogWarning("HTTP {Client} retry #{Attempt} after {Delay}s due to {Error}",
                                clientName, retryAttempt, timespan.TotalSeconds, outcome.Exception?.Message));
            });

            return services;
        }
    }
}
