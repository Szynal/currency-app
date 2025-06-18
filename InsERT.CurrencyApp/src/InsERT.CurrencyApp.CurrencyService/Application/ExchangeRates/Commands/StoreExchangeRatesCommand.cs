using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.CurrencyService.Infrastructure.Nbp;

namespace InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Commands;

public record StoreExchangeRatesCommand(NbpTable Table) : ICommand<int>;
