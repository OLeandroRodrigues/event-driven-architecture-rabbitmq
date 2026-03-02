using Xunit;
using EventDrivenArchitecture.RabbitMQ.Infrastructure.Messaging.Envelope;


namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.UnitTests.Messaging.Envelope;

public sealed class MessageMetadataFactoryTests
{
    private sealed class SampleEvent { }

    [Fact]
    public void Create_WhenNoIdsProvided_ShouldGenerateMessageIdAndCorrelationId()
    {
        var metadata = MessageMetadataFactory.Create<SampleEvent>();

        Assert.False(string.IsNullOrWhiteSpace(metadata.MessageId));
        Assert.False(string.IsNullOrWhiteSpace(metadata.CorrelationId));
        Assert.Equal(typeof(SampleEvent).FullName, metadata.MessageType);
    }

    [Fact]
    public void Create_WhenIdsProvided_ShouldUseProvidedValues()
    {
        var messageId = "msg-001";
        var correlationId = "corr-001";
        var occurredAt = new DateTime(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc);

        var metadata = MessageMetadataFactory.Create<SampleEvent>(
            messageId: messageId,
            correlationId: correlationId,
            occurredAtUtc: occurredAt);

        Assert.Equal(messageId, metadata.MessageId);
        Assert.Equal(correlationId, metadata.CorrelationId);
        Assert.Equal(occurredAt, metadata.OccurredAtUtc);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EnsureCorrelationId_WhenMissing_ShouldGenerate(string? correlationId)
    {
        var ensured = MessageMetadataFactory.EnsureCorrelationId(correlationId);

        Assert.False(string.IsNullOrWhiteSpace(ensured));
    }

    [Fact]
    public void NewId_ShouldBeHexLowercase_AndLength32()
    {
        var id = MessageMetadataFactory.NewId();

        Assert.Equal(32, id.Length);
        Assert.Matches("^[0-9a-f]{32}$", id);
    }
}
