using Xunit;
using EventDrivenArchitecture.RabbitMQ.Infrastructure.Observability.Correlation;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.UnitTests.Observability.Correlation;

public sealed class AsyncLocalCorrelationContextTests
{
    [Fact]
    public void CorrelationId_Default_ShouldBeEmptyString()
    {
        var ctx = new AsyncLocalCorrelationContext();
        Assert.Equal(string.Empty, ctx.CorrelationId);
    }

    [Fact]
    public void BeginScope_ShouldSetCorrelationId_AndRestorePreviousOnDispose()
    {
        var ctx = new AsyncLocalCorrelationContext();

        using (ctx.BeginScope("corr-1"))
        {
            Assert.Equal("corr-1", ctx.CorrelationId);

            using (ctx.BeginScope("corr-2"))
            {
                Assert.Equal("corr-2", ctx.CorrelationId);
            }

            Assert.Equal("corr-1", ctx.CorrelationId);
        }

        Assert.Equal(string.Empty, ctx.CorrelationId);
    }
}
