using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

namespace EventDrivenArchitecture.RabbitMQ.Application.UseCases;

/// <summary>
/// Applies Inbox Pattern semantics at the application boundary:
/// - If message was processed, it skips execution.
/// - Otherwise executes and marks as processed.
/// </summary>
public sealed class IdempotentUseCaseExecutor
{
    private readonly IInboxStore _inboxStore;
    private readonly IClock? _clock;

    public IdempotentUseCaseExecutor(IInboxStore inboxStore, IClock? clock = null)
    {
        _inboxStore = inboxStore ?? throw new ArgumentNullException(nameof(inboxStore));
        _clock = clock;
    }

    public async Task ExecuteAsync(
        string messageId,
        Func<CancellationToken, Task> useCase,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageId);
        ArgumentNullException.ThrowIfNull(useCase);

        if (await _inboxStore.HasBeenProcessedAsync(messageId, cancellationToken))
            return;

        await useCase(cancellationToken);

        var processedAt = _clock?.UtcNow ?? DateTime.UtcNow;
        await _inboxStore.MarkAsProcessedAsync(messageId, processedAt, cancellationToken);
    }
}