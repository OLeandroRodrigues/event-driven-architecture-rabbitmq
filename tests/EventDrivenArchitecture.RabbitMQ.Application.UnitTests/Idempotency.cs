using EventDrivenArchitecture.RabbitMQ.Application.UnitTests.Fakes;
using EventDrivenArchitecture.RabbitMQ.Application.UseCases;
using Xunit;

namespace EventDrivenArchitecture.RabbitMQ.Application.UnitTests.Idempotency;

public sealed class IdempotentUseCaseExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_WhenSameMessageIdArrivesTwice_ShouldRunUseCaseOnlyOnce()
    {
        // Arrange
        var inbox = new FakeInboxStore();
        var executor = new IdempotentUseCaseExecutor(inbox);

        var messageId = "msg-123";
        var executionCount = 0;

        Task UseCase(CancellationToken _)
        {
            executionCount++;
            return Task.CompletedTask;
        }

        // Act
        await executor.ExecuteAsync(messageId, UseCase);
        await executor.ExecuteAsync(messageId, UseCase);

        // Assert
        Assert.Equal(1, executionCount);
    }
}