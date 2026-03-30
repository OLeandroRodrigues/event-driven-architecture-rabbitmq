using System.Collections.Concurrent;
using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

namespace EventDrivenArchitecture.RabbitMQ.Application.UnitTests.Fakes;

public sealed class FakeInboxStore : IInboxStore
{
    private readonly ConcurrentDictionary<string, DateTime> _processed =
        new(StringComparer.Ordinal);

    public Task<bool> HasBeenProcessedAsync(
        string messageId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_processed.ContainsKey(messageId));
    }

    public Task MarkAsProcessedAsync(
        string messageId,
        DateTime processedAt,
        CancellationToken cancellationToken = default)
    {
        _processed[messageId] = processedAt;
        return Task.CompletedTask;
    }

    public bool Contains(string messageId) => _processed.ContainsKey(messageId);

    public DateTime? GetProcessedAt(string messageId)
        => _processed.TryGetValue(messageId, out var value) ? value : null;
}