namespace InsERT.CurrencyApp.TransactionService.Infrastructure.Health;

public interface IAppHealthService
{
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}
