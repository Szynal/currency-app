using InsERT.CurrencyApp.Abstractions.CQRS.Queries;
using InsERT.CurrencyApp.CurrencyService.Application.Queries;
using InsERT.CurrencyApp.CurrencyService.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace InsERT.CurrencyApp.CurrencyService.Application.Handlers;

public class GetCurrencyCodesHandler : IQueryHandler<GetCurrencyCodesQuery, IEnumerable<string>>
{
    private readonly CurrencyDbContext _dbContext;

    public GetCurrencyCodesHandler(CurrencyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<string>> HandleAsync(GetCurrencyCodesQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.ExchangeRates
            .Select(r => r.Code)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }
}
