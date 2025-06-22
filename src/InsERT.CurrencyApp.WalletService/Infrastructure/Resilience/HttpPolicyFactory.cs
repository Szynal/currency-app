using System.Net.Http;
using InsERT.CurrencyApp.Abstractions.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.CircuitBreaker;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.Resilience
{
    internal class HttpPolicyFactory : IHttpPolicyFactory
    {
        private readonly ILogger<HttpPolicyFactory> _logger;

        public HttpPolicyFactory(ILogger<HttpPolicyFactory> logger) =>
            _logger = logger;

        public IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy<TSettings>(TSettings settings)
            where TSettings : IRetryPolicySettings
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    settings.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(settings.BackoffSeconds, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        _logger.LogWarning("Retry #{Attempt} after {Delay}s due to {Error}",
                            retryAttempt, timespan.TotalSeconds, outcome.Exception?.Message);
                    });
        }

        public IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy<TSettings>(TSettings settings)
            where TSettings : IRetryPolicySettings
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: settings.CircuitBreakerFailures,
                    durationOfBreak: TimeSpan.FromSeconds(settings.CircuitBreakerBreakSeconds),
                    onBreak: (outcome, breakDelay) =>
                        _logger.LogWarning("Circuit broken for {Delay}s due to {Error}", breakDelay.TotalSeconds, outcome.Exception?.Message),
                    onReset: () =>
                        _logger.LogInformation("Circuit reset.")
                );
        }
    }
}
