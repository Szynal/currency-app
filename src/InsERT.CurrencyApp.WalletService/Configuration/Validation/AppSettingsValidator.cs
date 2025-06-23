using Microsoft.Extensions.Options;

namespace InsERT.CurrencyApp.WalletService.Configuration.Validation;

public sealed class AppSettingsValidator : IValidateOptions<AppSettings>
{
    public ValidateOptionsResult Validate(string? name, AppSettings? settings)
    {
        if (settings is null)
        {
            return ValidateOptionsResult.Fail("AppSettings section is missing or malformed.");
        }

        if (string.IsNullOrWhiteSpace(settings.WalletDbConnectionString))
        {
            return ValidateOptionsResult.Fail("ConnectionString is required in AppSettings.");
        }

        return ValidateOptionsResult.Success;
    }
}
