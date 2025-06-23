using Microsoft.Extensions.Options;

namespace InsERT.CurrencyApp.TransactionService.Configuration.Validation;

public class AppSettingsValidator : IValidateOptions<AppSettings>
{
    public ValidateOptionsResult Validate(string? name, AppSettings settings)
    {
        if (settings is null)
            return ValidateOptionsResult.Fail("AppSettings is null.");

        if (string.IsNullOrWhiteSpace(settings.TransactionDbConnectionString))
        {
            return ValidateOptionsResult.Fail("TransactionDbConnectionString is required and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(settings.WalletServiceBaseUrl))
        {
            return ValidateOptionsResult.Fail("WalletServiceBaseUrl is required and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(settings.CurrencyServiceBaseUrl))
        {
            return ValidateOptionsResult.Fail("CurrencyServiceBaseUrl is required and cannot be empty.");
        }

        return ValidateOptionsResult.Success;
    }
}
