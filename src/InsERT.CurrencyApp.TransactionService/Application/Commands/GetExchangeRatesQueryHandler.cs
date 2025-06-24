using InsERT.CurrencyApp.Abstractions.Currency.Models;
using InsERT.CurrencyApp.Abstractions.Currency.Queries;
using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;

namespace InsERT.CurrencyApp.TransactionService.Application.Commands;

public class GetExchangeRatesQueryHandler(ICurrencyServiceClient currencyServiceClient) : IQueryHandler<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>
{
    private readonly ICurrencyServiceClient _currencyServiceClient = currencyServiceClient;

    public async Task<IEnumerable<ExchangeRateDto>> HandleAsync(GetExchangeRatesQuery query, CancellationToken cancellationToken)
    {
        return await _currencyServiceClient.GetExchangeRatesAsync(query.Date, query.Code, cancellationToken);
    }
}
