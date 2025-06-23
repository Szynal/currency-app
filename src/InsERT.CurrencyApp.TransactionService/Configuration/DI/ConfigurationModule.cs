using InsERT.CurrencyApp.TransactionService.Configuration.Validation;
using Microsoft.Extensions.Options;

namespace InsERT.CurrencyApp.TransactionService.Configuration.DI;

public static class ConfigurationModule
{
    public static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AppSettings>()
            .Configure(options =>
            {
                options.TransactionDbConnectionString = configuration.GetConnectionString("TransactionDb")
                    ?? throw new InvalidOperationException("Connection string 'TransactionDb' not found.");

                options.WalletServiceBaseUrl = configuration.GetSection("Services")["WalletServiceBaseUrl"]
                    ?? throw new InvalidOperationException("WalletServiceBaseUrl not found in configuration.");

                options.CurrencyServiceBaseUrl = configuration.GetSection("Services")["CurrencyServiceBaseUrl"]
                    ?? throw new InvalidOperationException("CurrencyServiceBaseUrl not found in configuration.");
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<AppSettings>, AppSettingsValidator>();

        return services;
    }
}
