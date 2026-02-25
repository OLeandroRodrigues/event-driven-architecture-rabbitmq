using System.Collections.Concurrent;
using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.Idempotency;

/// <summary>
/// In-memory implementation of the Inbox Pattern.
/// Useful for local development and unit/integration tests.
/// 
/// IMPORTANT:
/// - Data is lost on process restart.
/// - This is not suitable for production.
/// </summary>
public sealed class InMemoryInboxStore : IInboxStore
{
    // messageId -> processedAt (UTC)
    private readonly ConcurrentDictionary<string, DateTime> _processed =
        new(StringComparer.Ordinal);

    public Task<bool> HasBeenProcessedAsync(
        string messageId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageId);

        // cancellationToken not used because this is in-memory and immediate,
        // but kept to match the interface contract.
        return Task.FromResult(_processed.ContainsKey(messageId));
    }

    public Task MarkAsProcessedAsync(
        string messageId,
        DateTime processedAt,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageId);

        // Ensure stored timestamps are in UTC to keep semantics consistent.
        if (processedAt.Kind == DateTimeKind.Unspecified)
            processedAt = DateTime.SpecifyKind(processedAt, DateTimeKind.Utc);
        else
            processedAt = processedAt.ToUniversalTime();

        // Idempotent write: if already exists, keep the earliest processedAt
        _processed.AddOrUpdate(
            messageId,
            processedAt,
            (_, existing) => existing <= processedAt ? existing : processedAt);

        return Task.CompletedTask;
    }
}