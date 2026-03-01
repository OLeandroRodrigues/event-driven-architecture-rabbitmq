using System.Threading;
using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.Observability.Correlation;

/// <summary>
/// AsyncLocal-based correlation context.
/// This allows correlation to flow through async calls within the same logical execution.
/// </summary>
public sealed class AsyncLocalCorrelationContext : ICorrelationContext
{
    private static readonly AsyncLocal<string?> _current = new();

    public string CorrelationId => _current.Value ?? string.Empty;

    public IDisposable BeginScope(string correlationId)
    {
        var prior = _current.Value;
        _current.Value = correlationId;

        return new Scope(() => _current.Value = prior);
    }

    private sealed class Scope : IDisposable
    {
        private readonly Action _onDispose;
        private int _disposed;

        public Scope(Action onDispose) => _onDispose = onDispose;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1) return;
            _onDispose();
        }
    }
}
