using System.Collections.Concurrent;
using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

namespace EventDrivenArchitecture.RabbitMQ.Application.UnitTests.Fakes;

public sealed class FakeInboxStore : IInboxStore
{
    private readonly ConcurrentDictionary<string, DateTime> _processed =
        new(StringComparer.Ordinal);

    public Task<bool> HasBeenProcessedAsync(string messageId, CancellationToken cancellationToken = default)
        => Task.FromResult(_processed.ContainsKey(messageId));

    public Task MarkAsProcessedAsync(string messageId, DateTime processedAt, CancellationToken cancellationToken = default)
    {
        _processed.TryAdd(messageId, processedAt);
        return Task.CompletedTask;
    }
}