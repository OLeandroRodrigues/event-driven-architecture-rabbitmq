using EventDrivenArchitecture.RabbitMQ.Infrastructure.Observability.Correlation;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.UnitTests.Fakes;

public sealed class FakeCorrelationScope : ICorrelationScope
{
    public string? LastCorrelationId { get; private set; }

    public int BeginScopeCallCount { get; private set; }

    public IDisposable BeginScope(string correlationId)
    {
        LastCorrelationId = correlationId;
        BeginScopeCallCount++;

        return new NoOpDisposable();
    }

    private sealed class NoOpDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}