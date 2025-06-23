namespace InsERT.CurrencyApp.Abstractions.Events;
public interface IEventSubscriber
{
    Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler, CancellationToken cancellationToken = default);
}