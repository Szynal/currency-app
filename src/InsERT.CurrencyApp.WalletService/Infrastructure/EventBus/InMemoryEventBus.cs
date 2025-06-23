using System.Collections.Concurrent;
using InsERT.CurrencyApp.Abstractions.Events;

namespace InsERT.CurrencyApp.WalletService.Infrastructure.EventBus;

public class InMemoryEventBus : IEventPublisher, IEventSubscriber
{
    private readonly ConcurrentDictionary<Type, List<Func<object, Task>>> _handlers = new();

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
    {
        if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            return Task.WhenAll(handlers.Select(h => h(@event)));
        }
        return Task.CompletedTask;
    }

    public Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler, CancellationToken cancellationToken = default)
    {
        var handlers = _handlers.GetOrAdd(typeof(TEvent), _ => []);
        handlers.Add(e => handler((TEvent)e));
        return Task.CompletedTask;
    }
}
