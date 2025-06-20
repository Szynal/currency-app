using Microsoft.Extensions.Options;

namespace InsERT.CurrencyApp.CurrencyService.Configuration.Validation;

public class AppSettingsValidator : IValidateOptions<AppSettings>
{
    public ValidateOptionsResult Validate(string? name, AppSettings settings)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            failures.Add("ConnectionString is required.");

        if (settings.FetchIntervalMinutes <= 0)
            failures.Add("FetchIntervalMinutes must be greater than 0.");

        if (settings.NbpClient is null)
            failures.Add("NbpClient section is required.");
        else
        {
            if (string.IsNullOrWhiteSpace(settings.NbpClient.BaseUrl))
                failures.Add("NbpClient.BaseUrl is required.");
            if (settings.NbpClient.RetryCount <= 0)
                failures.Add("NbpClient.RetryCount must be > 0.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
