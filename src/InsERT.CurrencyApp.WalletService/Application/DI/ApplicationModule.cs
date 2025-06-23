using FluentValidation;
using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.WalletService.Application.Behaviors;
using InsERT.CurrencyApp.WalletService.Application.Commands;
using InsERT.CurrencyApp.WalletService.Application.Commands.Validators;
using InsERT.CurrencyApp.WalletService.Application.DTOs;
using InsERT.CurrencyApp.WalletService.Application.Handlers;
using InsERT.CurrencyApp.WalletService.Application.Queries;
using InsERT.CurrencyApp.WalletService.Application.Queries.Handlers;

namespace InsERT.CurrencyApp.WalletService.Application.DI;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Dispatchery CQRS
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        // Command Handlers
        services.AddScoped<ICommandHandler<CreateWalletCommand, Guid>, CreateWalletCommandHandler>();
        services.AddScoped<ICommandHandler<ApplyWalletTransactionCommand, Unit>, ApplyWalletTransactionCommandHandler>();

        // Query Handlers
        services.AddScoped<IQueryHandler<GetWalletBalancesQuery, IReadOnlyCollection<WalletBalanceDto>>, GetWalletBalancesQueryHandler>();
        services.AddScoped<ICommandHandler<AddWalletBalanceCommand, Unit>, AddWalletBalanceCommandHandler>();

        // Validation
        services.AddScoped<IValidationBehavior, ValidationBehavior>();
        services.AddValidatorsFromAssemblyContaining<CreateWalletCommandValidator>();

        return services;
    }
}
