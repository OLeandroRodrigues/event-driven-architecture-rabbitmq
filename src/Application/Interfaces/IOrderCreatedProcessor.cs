using EventDrivenArchitecture.RabbitMQ.Domain.Events;

namespace EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

public interface IOrderCreatedProcessor
{
    Task ProcessAsync(OrderCreated @event,
                      CancellationToken cancellationToken = default);
}