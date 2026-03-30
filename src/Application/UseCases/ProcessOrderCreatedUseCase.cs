using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;
using EventDrivenArchitecture.RabbitMQ.Domain.Events;

namespace EventDrivenArchitecture.RabbitMQ.Application.UseCases;


public sealed class ProcessOrderCreatedUseCase
{
    private readonly IdempotentUseCaseExecutor _executor;
    private readonly IOrderCreatedProcessor _processor;

    public ProcessOrderCreatedUseCase(
        IdempotentUseCaseExecutor executor,
        IOrderCreatedProcessor processor)
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
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
            ct => _processor.ProcessAsync(@event, ct),
            cancellationToken);
    }
}