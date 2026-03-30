using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;
using EventDrivenArchitecture.RabbitMQ.Domain.Events;

namespace EventDrivenArchitecture.RabbitMQ.Application.UnitTests.Fakes;

public sealed class SpyOrderCreatedProcessor : IOrderCreatedProcessor
{
    public int ExecutionCount { get; private set; }

    public OrderCreated? LastEvent { get; private set; }

    public bool ShouldThrow { get; set; }

    public Task ProcessAsync(
        OrderCreated @event,
        CancellationToken cancellationToken = default)
    {
        if (ShouldThrow)
            throw new InvalidOperationException("Simulated processing failure.");

        ExecutionCount++;
        LastEvent = @event;

        return Task.CompletedTask;
    }
}