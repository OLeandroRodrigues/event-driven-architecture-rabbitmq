using System.Collections.Concurrent;
using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.Messaging.InMemory;

/// <summary>
/// In-memory event publisher for testing and local development.
/// </summary>
public sealed class InMemoryEventPublisher : IEventPublisher
{
    private readonly ConcurrentQueue<object> _events = new();

    public Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(@event);

        _events.Enqueue(@event);

        return Task.CompletedTask;
    }

    public IReadOnlyCollection<object> PublishedEvents
        => _events.ToArray();

    public IReadOnlyCollection<TEvent> PublishedEventsOfType<TEvent>()
        where TEvent : class
        => _events.OfType<TEvent>().ToArray();
}