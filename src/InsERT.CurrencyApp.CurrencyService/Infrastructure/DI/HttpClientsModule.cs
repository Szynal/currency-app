using InsERT.CurrencyApp.Abstractions.Http;
using InsERT.CurrencyApp.CurrencyService.Configuration;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.DI;

public static class HttpClientsModule
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        services
            .AddConfiguredHttpClient<NbpClientSettings>("AppSettings:NbpClient")
            .AddScoped<INbpCurrencyClient, NbpCurrencyClient>();

        return services;
    }

    private static IServiceCollection AddConfiguredHttpClient<TSettings>(
        this IServiceCollection services,
        string sectionName)
        where TSettings : class, IRetryPolicySettings, new()
    {
        services.AddOptions<TSettings>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient(typeof(TSettings).Name, (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<TSettings>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        })
        .AddPolicyHandler((sp, _) =>
        {
            var settings = sp.GetRequiredService<IOptions<TSettings>>().Value;
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(TSettings).Name);

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    settings.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(settings.BackoffSeconds, retryAttempt)),
                    (outcome, timespan, retryAttempt, _) =>
                    {
                        logger.LogWarning(
                            "Retry {RetryAttempt} after {Delay}s due to {Error}",
                            retryAttempt,
                            timespan.TotalSeconds,
                            outcome.Exception?.Message ?? "Unknown error");
                    });
        });

        return services;
    }
}
