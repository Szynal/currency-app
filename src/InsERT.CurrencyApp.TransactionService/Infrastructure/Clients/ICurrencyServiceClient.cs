using InsERT.CurrencyApp.Abstractions.Currency.Models;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;
public interface ICurrencyServiceClient
{
    Task<IEnumerable<ExchangeRateDto>> GetExchangeRatesAsync(DateOnly? date, string? code, CancellationToken cancellationToken);
}