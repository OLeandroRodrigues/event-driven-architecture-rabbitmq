using EventDrivenArchitecture.RabbitMQ.Domain.Events;
using EventDrivenArchitecture.RabbitMQ.Infrastructure.Messaging.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure.UnitTests.Messaging.InMemory;

public sealed class InMemoryEventPublisherTests
{
    [Fact]
    public async Task PublishAsync_ShouldStoreEvent()
    {
        var publisher = new InMemoryEventPublisher();

        var evt = new OrderCreated("1", "2", 10, DateTime.UtcNow);

        await publisher.PublishAsync(evt);

        Assert.Single(publisher.PublishedEvents);
    }
}

