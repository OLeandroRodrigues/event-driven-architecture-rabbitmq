using System.Security.Cryptography;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.Messaging.Envelope;

/// <summary>
/// Creates consistent metadata (MessageId, CorrelationId, timestamps) for messages.
/// </summary>
public static class MessageMetadataFactory
{
    public static MessageMetadata Create<TMessage>(
        string? messageId = null,
        string? correlationId = null,
        DateTime? occurredAtUtc = null)
        where TMessage : class
    {
        var typeName = typeof(TMessage).FullName ?? typeof(TMessage).Name;

        return new MessageMetadata(
            MessageId: string.IsNullOrWhiteSpace(messageId) ? NewId() : messageId!,
            CorrelationId: string.IsNullOrWhiteSpace(correlationId) ? NewId() : correlationId!,
            OccurredAtUtc: (occurredAtUtc ?? DateTime.UtcNow),
            MessageType: typeName
        );
    }

    public static string EnsureCorrelationId(string? correlationId)
        => string.IsNullOrWhiteSpace(correlationId) ? NewId() : correlationId!;

    /// <summary>
    /// Generates a compact identifier (hex string).
    /// </summary>
    public static string NewId()
    {
        Span<byte> bytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
