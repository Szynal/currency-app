using FluentValidation;
using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;
using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.WalletService.Application.Behaviors;
using InsERT.CurrencyApp.WalletService.Application.Commands;
using InsERT.CurrencyApp.WalletService.Application.Commands.Validators;
using InsERT.CurrencyApp.WalletService.Application.DTOs;
using InsERT.CurrencyApp.WalletService.Application.Queries;
using InsERT.CurrencyApp.WalletService.Application.Queries.Handlers;
using CommandDispatcher = InsERT.CurrencyApp.Abstractions.CQRS.CommandDispatcher;

namespace InsERT.CurrencyApp.WalletService.Application.DI;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped<ICommandHandler<CreateWalletCommand, Guid>, CreateWalletCommandHandler>();

        services.AddScoped<IQueryHandler<GetWalletBalancesQuery, IReadOnlyCollection<WalletBalanceDto>>, GetWalletBalancesQueryHandler>();

        services.AddScoped<IValidationBehavior, ValidationBehavior>();
        services.AddValidatorsFromAssemblyContaining<CreateWalletCommandValidator>();

        return services;
    }
}
