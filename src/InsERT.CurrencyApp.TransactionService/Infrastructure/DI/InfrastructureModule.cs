using InsERT.CurrencyApp.Abstractions.CQRS;
using InsERT.CurrencyApp.Abstractions.CQRS.Commands;
using InsERT.CurrencyApp.TransactionService.Application.Commands;
using InsERT.CurrencyApp.TransactionService.Application.Commands.Handlers;
using InsERT.CurrencyApp.TransactionService.Domain;
using InsERT.CurrencyApp.TransactionService.Domain.Repositories;
using InsERT.CurrencyApp.TransactionService.Infrastructure.Clients;
using InsERT.CurrencyApp.TransactionService.Infrastructure.DataAccess;
using InsERT.CurrencyApp.TransactionService.Infrastructure.HostedServices;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.DI;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TransactionDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("TransactionDb");
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var walletServiceUrl = configuration.GetValue<string>("Services:WalletServiceBaseUrl")
                               ?? throw new InvalidOperationException("WalletServiceBaseUrl not configured.");

        services.AddHttpClient<IWalletServiceClient, WalletServiceHttpClient>(client =>
        {
            client.BaseAddress = new Uri(walletServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        var currencyServiceUrl = configuration.GetValue<string>("Services:CurrencyServiceBaseUrl")
                                ?? throw new InvalidOperationException("CurrencyServiceBaseUrl not configured.");

        services.AddHttpClient<ICurrencyServiceClient, CurrencyServiceHttpClient>(client =>
        {
            client.BaseAddress = new Uri(currencyServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddScoped<ICommandHandler<ApplyTransactionCommand, Unit>, ApplyTransactionCommandHandler>();

        return services;
    }
}
