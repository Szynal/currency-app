﻿using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.Abstractions.Currency.Models;
using InsERT.CurrencyApp.Abstractions.Currency.Queries;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Commands;
using InsERT.CurrencyApp.CurrencyService.Application.ExchangeRates.Handlers;
using InsERT.CurrencyApp.CurrencyService.Application.Handlers;
using InsERT.CurrencyApp.CurrencyService.Application.Queries;
using InsERT.CurrencyApp.CurrencyService.Application.Services;

namespace InsERT.CurrencyApp.CurrencyService.Application.DI;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped<ICurrencyRateFetchJob, CurrencyRateFetchJob>();
        services.AddScoped<ICommandHandler<StoreExchangeRatesCommand, int>, StoreExchangeRatesHandler>();
        services.AddScoped<IQueryHandler<GetExchangeRatesQuery, IEnumerable<ExchangeRateDto>>, GetExchangeRatesHandler>();
        services.AddScoped<IQueryHandler<GetCurrencyCodesQuery, IEnumerable<string>>, GetCurrencyCodesHandler>();

        return services;
    }
}
