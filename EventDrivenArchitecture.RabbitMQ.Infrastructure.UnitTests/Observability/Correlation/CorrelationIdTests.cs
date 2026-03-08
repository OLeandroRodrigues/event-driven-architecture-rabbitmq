using Xunit;
using EventDrivenArchitecture.RabbitMQ.Infrastructure.Observability.Correlation;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.UnitTests.Observability.Correlation;

public sealed class CorrelationIdTests
{
    [Fact]
    public void New_ShouldReturnNonEmpty()
    {
        var id = CorrelationId.New();
        Assert.False(string.IsNullOrWhiteSpace(id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ensure_WhenMissing_ShouldGenerate(string? correlationId)
    {
        var ensured = CorrelationId.Ensure(correlationId);
        Assert.False(string.IsNullOrWhiteSpace(ensured));
    }

    [Fact]
    public void Ensure_WhenProvided_ShouldReturnSame()
    {
        var ensured = CorrelationId.Ensure("corr-xyz");
        Assert.Equal("corr-xyz", ensured);
    }
}
