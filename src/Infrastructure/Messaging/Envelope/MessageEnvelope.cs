namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.Messaging.Envelope;

/// <summary>
/// Broker-agnostic envelope for transporting messages.
/// Payload format is intentionally left open (JSON, protobuf, etc.).
/// </summary>
public sealed record MessageEnvelope(
    MessageMetadata Metadata,
    ReadOnlyMemory<byte> Payload
);
