using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Composition root:
        // - Register Application services (use cases)
        // - Register Infrastructure services (RabbitMQ, persistence, idempotency, observability)
        // - Register hosted consumers
    })
    .Build();

await host.RunAsync();
