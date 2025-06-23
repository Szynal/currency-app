namespace InsERT.CurrencyApp.Abstractions.Events;
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
}
