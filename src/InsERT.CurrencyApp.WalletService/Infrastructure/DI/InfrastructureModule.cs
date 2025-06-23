namespace InsERT.CurrencyApp.WalletService.Infrastructure.DI;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddDataAccess();
    }
}
