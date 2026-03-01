namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.Messaging.Envelope;

/// <summary>
/// Broker-agnostic metadata that travels with every message.
/// Keep this stable: it becomes the contract for observability and reliability.
/// </summary>
public sealed record MessageMetadata(
    string MessageId,
    string CorrelationId,
    DateTime OccurredAtUtc,
    string MessageType
);
