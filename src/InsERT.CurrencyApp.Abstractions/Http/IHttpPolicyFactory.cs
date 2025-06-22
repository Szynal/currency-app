using Polly;

namespace InsERT.CurrencyApp.Abstractions.Http;

public interface IHttpPolicyFactory
{
    IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy<TSettings>(TSettings settings)
        where TSettings : IRetryPolicySettings;

    IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy<TSettings>(TSettings settings)
        where TSettings : IRetryPolicySettings;
}
