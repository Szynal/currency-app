using InsERT.CurrencyApp.Abstractions.Http;
using System.ComponentModel.DataAnnotations;

namespace InsERT.CurrencyApp.CurrencyService.Configuration;

public class NbpClientSettings : IRetryPolicySettings
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "NbpClient.BaseUrl is required.")]
    [Url(ErrorMessage = "NbpClient.BaseUrl must be a valid URL.")]
    public string BaseUrl { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "NbpClient.TableBEndpoint is required.")]
    public string TableBEndpoint { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "NbpClient.RetryCount must be > 0.")]
    public int RetryCount { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "NbpClient.BackoffSeconds must be > 0.")]
    public int BackoffSeconds { get; set; }

    public string? ClientName { get; set; }
}
