namespace EventDrivenArchitecture.RabbitMQ.Application.Interfaces;
public interface IInboxStore
{
    Task<bool> HasBeenProcessedAsync(
        string messageId,
        CancellationToken cancellationToken = default);

    Task MarkAsProcessedAsync(
        string messageId,
        DateTime processedAt,
        CancellationToken cancellationToken = default);
}