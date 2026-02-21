namespace EventDrivenArchitecture.RabbitMQ.Application.Interfaces;

public interface ICorrelationContext
{   
    string CorrelationId { get; }
}