using InsERT.CurrencyApp.CurrencyService.Configuration;

namespace InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

public static class NbpUriFactory
{
    public static Uri BuildTableEndpoint(NbpClientSettings config)
    {
        if (string.IsNullOrWhiteSpace(config.BaseUrl))
            ThrowInvalidConfig(nameof(config.BaseUrl));

        if (string.IsNullOrWhiteSpace(config.TableBEndpoint))
            ThrowInvalidConfig(nameof(config.TableBEndpoint));

        var combinedUrl = $"{config.BaseUrl}{config.TableBEndpoint}";

        if (!Uri.TryCreate(combinedUrl, UriKind.Absolute, out var fullUri))
            throw new ArgumentException($"Invalid full URL: {combinedUrl}");

        return fullUri;
    }

    private static void ThrowInvalidConfig(string key) =>
        throw new ArgumentException($"NBP client configuration '{key}' is required.");
}
