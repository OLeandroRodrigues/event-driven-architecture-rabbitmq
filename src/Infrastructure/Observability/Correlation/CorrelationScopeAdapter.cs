using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.Observability.Correlation;

/// <summary>
/// Adapts the AsyncLocal context to an infrastructure-only scope API.
/// Keeps Application's ICorrelationContext minimal (getter only).
/// </summary>
public sealed class CorrelationScopeAdapter : ICorrelationScope
{
    private readonly AsyncLocalCorrelationContext _context;

    public CorrelationScopeAdapter(ICorrelationContext context)
    {
        _context = context as AsyncLocalCorrelationContext
            ?? throw new InvalidOperationException("Expected AsyncLocalCorrelationContext registration.");
    }

    public IDisposable BeginScope(string correlationId) => _context.BeginScope(correlationId);
}
