namespace EventDrivenArchitecture.RabbitMQ.Domain.Events;

public sealed record OrderProcessed(
    string OrderId,
    DateTime ProcessedAtUtc
);