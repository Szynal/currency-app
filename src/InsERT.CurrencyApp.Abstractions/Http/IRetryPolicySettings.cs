namespace InsERT.CurrencyApp.Abstractions.Http;

public interface IRetryPolicySettings
{
    string BaseUrl { get; }
    int RetryCount { get; }
    int BackoffSeconds { get; }
}
