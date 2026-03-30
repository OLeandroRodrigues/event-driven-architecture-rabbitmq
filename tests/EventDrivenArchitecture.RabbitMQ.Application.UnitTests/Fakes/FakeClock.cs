using EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

namespace EventDrivenArchitecture.RabbitMQ.Application.UnitTests.Fakes;

public sealed class FakeClock : IClock
{
    public DateTime UtcNow { get; set; }
}