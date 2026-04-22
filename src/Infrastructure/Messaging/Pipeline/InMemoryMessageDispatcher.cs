using EventDrivenArchitecture.RabbitMQ.Application.UseCases;
using EventDrivenArchitecture.RabbitMQ.Domain.Events;
using EventDrivenArchitecture.RabbitMQ.Infrastructure.Observability.Correlation;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.Messaging.Pipeline;

/// <summary>
/// Simulates a message consumer pipeline without a real message broker.
/// Useful for validating application flow, idempotency, and correlation handling
/// before introducing RabbitMQ infrastructure.
/// </summary>
public sealed class InMemoryMessageDispatcher
{
    private readonly ProcessOrderCreatedUseCase _useCase;
    private readonly ICorrelationScope _correlationScope;

    public InMemoryMessageDispatcher(
        ProcessOrderCreatedUseCase useCase,
        ICorrelationScope correlationScope)
    {
        _useCase = useCase ?? throw new ArgumentNullException(nameof(useCase));
        _correlationScope = correlationScope ?? throw new ArgumentNullException(nameof(correlationScope));
    }

    public async Task DispatchAsync(
        string messageId,
        string? correlationId,
        OrderCreated @event,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageId);
        ArgumentNullException.ThrowIfNull(@event);

        var ensuredCorrelationId = CorrelationId.Ensure(correlationId);

        using (_correlationScope.BeginScope(ensuredCorrelationId))
        {
            await _useCase.ExecuteAsync(
                messageId,
                @event,
                cancellationToken);
        }
    }
}