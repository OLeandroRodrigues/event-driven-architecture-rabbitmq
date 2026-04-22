using EventDrivenArchitecture.RabbitMQ.Application.UnitTests.Fakes;
using EventDrivenArchitecture.RabbitMQ.Application.UseCases;
using EventDrivenArchitecture.RabbitMQ.Domain.Events;
using EventDrivenArchitecture.RabbitMQ.Infrastructure.Messaging.Pipeline;
using EventDrivenArchitecture.RabbitMQ.Infrastructure.UnitTests.Fakes;
using Xunit;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.UnitTests.Messaging.Pipeline;

public sealed class InMemoryMessageDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_WhenMessageIsNew_ShouldExecuteUseCase()
    {
        // Arrange
        var inbox = new FakeInboxStore();
        var clock = new FakeClock
        {
            UtcNow = new DateTime(2026, 1, 23, 10, 0, 0, DateTimeKind.Utc)
        };
        var processor = new SpyOrderCreatedProcessor();
        var executor = new IdempotentUseCaseExecutor(inbox, clock);
        var useCase = new ProcessOrderCreatedUseCase(executor, processor);
        var correlationScope = new FakeCorrelationScope();

        var dispatcher = new InMemoryMessageDispatcher(useCase, correlationScope);

        var messageId = "msg-001";
        var correlationId = "corr-001";
        var @event = new OrderCreated(
            OrderId: "order-001",
            CustomerId: "customer-001",
            TotalAmount: 100m,
            OccurredAtUtc: clock.UtcNow);

        // Act
        await dispatcher.DispatchAsync(messageId, correlationId, @event);

        // Assert
        Assert.Equal(1, processor.ExecutionCount);
    }

    [Fact]
    public async Task DispatchAsync_WhenMessageIsDuplicated_ShouldExecuteOnlyOnce()
    {
        // Arrange
        var inbox = new FakeInboxStore();
        var clock = new FakeClock
        {
            UtcNow = new DateTime(2026, 1, 23, 10, 0, 0, DateTimeKind.Utc)
        };
        var processor = new SpyOrderCreatedProcessor();
        var executor = new IdempotentUseCaseExecutor(inbox, clock);
        var useCase = new ProcessOrderCreatedUseCase(executor, processor);
        var correlationScope = new FakeCorrelationScope();

        var dispatcher = new InMemoryMessageDispatcher(useCase, correlationScope);

        var messageId = "msg-duplicate";
        var @event = new OrderCreated(
            OrderId: "order-002",
            CustomerId: "customer-002",
            TotalAmount: 200m,
            OccurredAtUtc: clock.UtcNow);

        // Act
        await dispatcher.DispatchAsync(messageId, "corr-001", @event);
        await dispatcher.DispatchAsync(messageId, "corr-001", @event);

        // Assert
        Assert.Equal(1, processor.ExecutionCount);
    }

    [Fact]
    public async Task DispatchAsync_WhenCorrelationIdIsProvided_ShouldUseIt()
    {
        // Arrange
        var inbox = new FakeInboxStore();
        var clock = new FakeClock
        {
            UtcNow = DateTime.UtcNow
        };
        var processor = new SpyOrderCreatedProcessor();
        var executor = new IdempotentUseCaseExecutor(inbox, clock);
        var useCase = new ProcessOrderCreatedUseCase(executor, processor);
        var correlationScope = new FakeCorrelationScope();

        var dispatcher = new InMemoryMessageDispatcher(useCase, correlationScope);

        var @event = new OrderCreated(
            OrderId: "order-003",
            CustomerId: "customer-003",
            TotalAmount: 300m,
            OccurredAtUtc: clock.UtcNow);

        // Act
        await dispatcher.DispatchAsync("msg-003", "corr-provided", @event);

        // Assert
        Assert.Equal("corr-provided", correlationScope.LastCorrelationId);
        Assert.Equal(1, correlationScope.BeginScopeCallCount);
    }

    [Fact]
    public async Task DispatchAsync_WhenCorrelationIdIsMissing_ShouldGenerateOne()
    {
        // Arrange
        var inbox = new FakeInboxStore();
        var clock = new FakeClock
        {
            UtcNow = DateTime.UtcNow
        };
        var processor = new SpyOrderCreatedProcessor();
        var executor = new IdempotentUseCaseExecutor(inbox, clock);
        var useCase = new ProcessOrderCreatedUseCase(executor, processor);
        var correlationScope = new FakeCorrelationScope();

        var dispatcher = new InMemoryMessageDispatcher(useCase, correlationScope);

        var @event = new OrderCreated(
            OrderId: "order-004",
            CustomerId: "customer-004",
            TotalAmount: 400m,
            OccurredAtUtc: clock.UtcNow);

        // Act
        await dispatcher.DispatchAsync("msg-004", null, @event);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(correlationScope.LastCorrelationId));
        Assert.Equal(1, correlationScope.BeginScopeCallCount);
    }
}