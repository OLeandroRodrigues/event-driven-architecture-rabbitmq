using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;
using EventDrivenArchitecture.RabbitMQ.Domain.Events;

namespace EventDrivenArchitecture.RabbitMQ.Application.UseCases;

public sealed class ProcessOrderCreatedUseCase
{
    private readonly IdempotentUseCaseExecutor _executor;
    private readonly IOrderCreatedProcessor _processor;
    private readonly IEventPublisher _eventPublisher;
    private readonly IClock _clock;

    public ProcessOrderCreatedUseCase(
        IdempotentUseCaseExecutor executor,
        IOrderCreatedProcessor processor,
        IEventPublisher eventPublisher,
        IClock clock)
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public Task ExecuteAsync(
        string messageId,
        OrderCreated @event,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageId);
        ArgumentNullException.ThrowIfNull(@event);

        return _executor.ExecuteAsync(
            messageId,
            async ct =>
            {
                await _processor.ProcessAsync(@event, ct);

                var processedEvent = new OrderProcessed(
                    OrderId: @event.OrderId,
                    ProcessedAtUtc: _clock.UtcNow);

                await _eventPublisher.PublishAsync(
                    processedEvent,
                    ct);
            },
            cancellationToken);
    }
}