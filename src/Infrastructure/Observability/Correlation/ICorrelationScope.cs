namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.Observability.Correlation;

/// <summary>
/// Infrastructure-only helper to manage correlation scopes.
/// Application layer should not depend on this.
/// </summary>
public interface ICorrelationScope
{
    IDisposable BeginScope(string correlationId);
}
