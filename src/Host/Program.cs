using EventDrivenArchitecture.RabbitMQ.Application;
using EventDrivenArchitecture.RabbitMQ.Infrastructure;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AppApplication();
        services.AddInfraestructure();

        // Composition root:
        // - Register Application services (use cases)
        // - Register Infrastructure services (RabbitMQ, persistence, idempotency, observability)
        // - Register hosted consumers
    })
    .Build();

await host.RunAsync();
