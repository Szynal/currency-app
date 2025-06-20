using InsERT.CurrencyApp.Abstractions.CQRS.Queries;

namespace InsERT.CurrencyApp.Abstractions.CQRS.Dispatcher;

public interface IQueryDispatcher
{
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;
}
