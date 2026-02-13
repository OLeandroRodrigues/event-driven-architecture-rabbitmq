using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDrivenArchitecture.RabbitMQ.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraestructure(this IServiceCollection services) {

            // Register infrastructure services:
            // - Messaging (RabbitMQ)
            // - Idempotency store (Inbox)
            // - Observability
            // - Persistence

            // Example placeholders (to be implemented later):
            // services.AddRabbitMqMessaging(configuration);
            // services.AddInboxStore(configuration);
            // services.AddObservability(configuration);

            return services; 
        }
    }
}
