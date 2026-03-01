using EventDrivenArchitecture.RabbitMQ.Infrastructure.Messaging.Envelope;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.Observability.Correlation;

/// <summary>
/// Helpers for correlation ids.
/// </summary>
public static class CorrelationId
{
    public static string New() => MessageMetadataFactory.NewId();

    public static string Ensure(string? correlationId)
        => MessageMetadataFactory.EnsureCorrelationId(correlationId);
}
