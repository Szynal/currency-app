using FluentValidation;
using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.TransactionService.Application.Commands;
using InsERT.CurrencyApp.TransactionService.Application.Commands.Handlers;
using InsERT.CurrencyApp.TransactionService.Application.Validators;
using InsERT.CurrencyApp.TransactionService.Configuration;
using InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;
using CommandDispatcher = InsERT.CurrencyApp.TransactionService.Infrastructure.Dispatcher.CommandDispatcher;

namespace InsERT.CurrencyApp.TransactionService.Application.DI;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>()
                          ?? throw new InvalidOperationException("AppSettings section is missing or invalid.");

        services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped<ICommandHandler<ApplyTransactionCommand, Unit>, ApplyTransactionCommandHandler>();
        services.AddScoped<ICommandHandler<CreateDepositCommand, Unit>, CreateDepositCommandHandler>();
        services.AddScoped<ICommandHandler<CreateWithdrawCommand, Unit>, CreateWithdrawCommandHandler>();
        services.AddScoped<ICommandHandler<CreateConversionCommand, Unit>, CreateConversionCommandHandler>();

        services.AddScoped<IValidator<CreateDepositCommand>, CreateDepositCommandValidator>();
        services.AddScoped<IValidator<CreateWithdrawCommand>, CreateWithdrawCommandValidator>();
        services.AddScoped<IValidator<CreateConversionCommand>, CreateConversionCommandValidator>();

        services.AddHttpClient<IWalletServiceClient, WalletServiceHttpClient>(client =>
        {
            client.BaseAddress = new Uri(appSettings.WalletServiceBaseUrl);
        });

        services.AddHttpClient<ICurrencyServiceClient, CurrencyServiceHttpClient>(client =>
        {
            client.BaseAddress = new Uri(appSettings.CurrencyServiceBaseUrl);
        });

        return services;
    }
}
