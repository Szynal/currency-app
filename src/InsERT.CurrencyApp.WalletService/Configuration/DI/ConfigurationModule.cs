using InsERT.CurrencyApp.WalletService.Configuration.Validation;
using Microsoft.Extensions.Options;

namespace InsERT.CurrencyApp.WalletService.Configuration.DI;

public static class ConfigurationModule
{
    public static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<AppSettings>()
            .Bind(configuration.GetSection("AppSettings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<AppSettings>, AppSettingsValidator>();

        return services;
    }
}
