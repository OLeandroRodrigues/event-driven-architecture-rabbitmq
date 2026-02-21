namespace EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

public interface IClock
{
    DateTime UtcNow { get; }
}