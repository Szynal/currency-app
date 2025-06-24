using InsERT.CurrencyApp.Abstractions.CQRS.Queries;

namespace InsERT.CurrencyApp.CurrencyService.Application.Queries;

public record GetCurrencyCodesQuery() : IQuery<IEnumerable<string>>;
