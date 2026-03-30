namespace EventDrivenArchitecture.RabbitMQ.Domain.Events;

public sealed record OrderCreated(
    string OrderId,
    string CustomerId,
    decimal TotalAmount,
    DateTime OccurredAtUtc
);