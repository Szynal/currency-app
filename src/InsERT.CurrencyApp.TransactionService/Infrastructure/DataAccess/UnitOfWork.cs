using InsERT.CurrencyApp.TransactionService.Domain;

namespace InsERT.CurrencyApp.TransactionService.Infrastructure.DataAccess;

public class UnitOfWork(TransactionDbContext dbContext) : IUnitOfWork
{
    private readonly TransactionDbContext _dbContext = dbContext;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
