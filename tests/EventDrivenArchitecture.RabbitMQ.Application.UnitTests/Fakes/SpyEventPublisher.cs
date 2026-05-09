using System.Collections.Concurrent;
using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

namespace EventDrivenArchitecture.RabbitMQ.Application.UnitTests.Fakes;

public sealed class SpyEventPublisher : IEventPublisher
{
    private readonly ConcurrentQueue<object> _publishedEvents = new();

    public Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(@event);

        _publishedEvents.Enqueue(@event);

        return Task.CompletedTask;
    }

    public IReadOnlyCollection<TEvent> PublishedEventsOfType<TEvent>()
        where TEvent : class
    {
        return _publishedEvents.OfType<TEvent>().ToArray();
    }
}