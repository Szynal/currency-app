using FluentValidation;
using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.TransactionService.Application.Commands;
using InsERT.CurrencyApp.TransactionService.Application.Commands.Handlers;
using InsERT.CurrencyApp.TransactionService.Application.Validators;


namespace InsERT.CurrencyApp.TransactionService.Application.DI;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped<ICommandHandler<ApplyTransactionCommand, Unit>, ApplyTransactionCommandHandler>();
        services.AddScoped<IValidator<ApplyTransactionCommand>, ApplyTransactionCommandValidator>();

        return services;
    }
}
