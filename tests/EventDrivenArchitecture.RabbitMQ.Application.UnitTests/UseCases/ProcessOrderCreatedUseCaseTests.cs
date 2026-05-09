using EventDrivenArchitecture.RabbitMQ.Application.UseCases;
using EventDrivenArchitecture.RabbitMQ.Application.UnitTests.Fakes;
using EventDrivenArchitecture.RabbitMQ.Domain.Events;
using Xunit;

namespace EventDrivenArchitecture.RabbitMQ.Application.UnitTests.UseCases;

public sealed class ProcessOrderCreatedUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenMessageIdIsNew_ShouldProcessEvent()
    {
        // Arrange
        var inbox = new FakeInboxStore();
        var clock = new FakeClock
        {
            UtcNow = new DateTime(2026, 1, 23, 10, 0, 0, DateTimeKind.Utc)
        };
        var processor = new SpyOrderCreatedProcessor();
        var publisher = new SpyEventPublisher();

        var executor = new IdempotentUseCaseExecutor(inbox, clock);
        var useCase = new ProcessOrderCreatedUseCase(
            executor,
            processor,
            publisher,
            clock);

        var messageId = "msg-001";
        var @event = new OrderCreated(
            OrderId: "order-123",
            CustomerId: "customer-456",
            TotalAmount: 99.90m,
            OccurredAtUtc: clock.UtcNow);

        // Act
        await useCase.ExecuteAsync(messageId, @event);

        // Assert
        Assert.Equal(1, processor.ExecutionCount);
        Assert.Equal(@event, processor.LastEvent);
    }

    [Fact]
    public async Task ExecuteAsync_WhenMessageIdIsDuplicated_ShouldProcessOnlyOnce()
    {
        // Arrange
        var inbox = new FakeInboxStore();
        var clock = new FakeClock
        {
            UtcNow = new DateTime(2026, 1, 23, 10, 0, 0, DateTimeKind.Utc)
        };
        var processor = new SpyOrderCreatedProcessor();
        var publisher = new SpyEventPublisher();

        var executor = new IdempotentUseCaseExecutor(inbox, clock);
        var useCase = new ProcessOrderCreatedUseCase(
            executor,
            processor,
            publisher,
            clock);

        var messageId = "msg-duplicate";
        var @event = new OrderCreated(
            OrderId: "order-123",
            CustomerId: "customer-456",
            TotalAmount: 99.90m,
            OccurredAtUtc: clock.UtcNow);

        // Act
        await useCase.ExecuteAsync(messageId, @event);
        await useCase.ExecuteAsync(messageId, @event);

        // Assert
        Assert.Equal(1, processor.ExecutionCount);
        Assert.Single(publisher.PublishedEventsOfType<OrderProcessed>());
    }

    [Fact]
    public async Task ExecuteAsync_AfterSuccessfulProcessing_ShouldMarkMessageAsProcessed()
    {
        // Arrange
        var inbox = new FakeInboxStore();
        var clock = new FakeClock
        {
            UtcNow = new DateTime(2026, 1, 23, 11, 30, 0, DateTimeKind.Utc)
        };
        var processor = new SpyOrderCreatedProcessor();
        var publisher = new SpyEventPublisher();

        var executor = new IdempotentUseCaseExecutor(inbox, clock);
        var useCase = new ProcessOrderCreatedUseCase(
            executor,
            processor,
            publisher,
            clock);

        var messageId = "msg-processed";
        var @event = new OrderCreated(
            OrderId: "order-789",
            CustomerId: "customer-999",
            TotalAmount: 150.00m,
            OccurredAtUtc: clock.UtcNow);

        // Act
        await useCase.ExecuteAsync(messageId, @event);

        // Assert
        Assert.True(inbox.Contains(messageId));
        Assert.Equal(clock.UtcNow, inbox.GetProcessedAt(messageId));
    }

    [Fact]
    public async Task ExecuteAsync_WhenProcessorFails_ShouldNotMarkMessageAsProcessed()
    {
        // Arrange
        var inbox = new FakeInboxStore();
        var clock = new FakeClock
        {
            UtcNow = new DateTime(2026, 1, 23, 12, 0, 0, DateTimeKind.Utc)
        };
        var processor = new SpyOrderCreatedProcessor
        {
            ShouldThrow = true
        };
        var publisher = new SpyEventPublisher();

        var executor = new IdempotentUseCaseExecutor(inbox, clock);
        var useCase = new ProcessOrderCreatedUseCase(
            executor,
            processor,
            publisher,
            clock);

        var messageId = "msg-failure";
        var @event = new OrderCreated(
            OrderId: "order-fail",
            CustomerId: "customer-fail",
            TotalAmount: 200.00m,
            OccurredAtUtc: clock.UtcNow);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => useCase.ExecuteAsync(messageId, @event));

        Assert.False(inbox.Contains(messageId));
        Assert.Equal(0, processor.ExecutionCount);
        Assert.Empty(publisher.PublishedEventsOfType<OrderProcessed>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenMessageIdIsNew_ShouldPublishOrderProcessed()
    {
        // Arrange
        var inbox = new FakeInboxStore();
        var clock = new FakeClock
        {
            UtcNow = new DateTime(2026, 1, 23, 13, 0, 0, DateTimeKind.Utc)
        };
        var processor = new SpyOrderCreatedProcessor();
        var publisher = new SpyEventPublisher();

        var executor = new IdempotentUseCaseExecutor(inbox, clock);
        var useCase = new ProcessOrderCreatedUseCase(
            executor,
            processor,
            publisher,
            clock);

        var messageId = "msg-publish";
        var @event = new OrderCreated(
            OrderId: "order-publish",
            CustomerId: "customer-001",
            TotalAmount: 300.00m,
            OccurredAtUtc: clock.UtcNow);

        // Act
        await useCase.ExecuteAsync(messageId, @event);

        // Assert
        var publishedEvent = Assert.Single(
            publisher.PublishedEventsOfType<OrderProcessed>());

        Assert.Equal(@event.OrderId, publishedEvent.OrderId);
        Assert.Equal(clock.UtcNow, publishedEvent.ProcessedAtUtc);
    }
}