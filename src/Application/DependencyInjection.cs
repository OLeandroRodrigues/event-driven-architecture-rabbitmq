using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenArchitecture.RabbitMQ.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AppApplication(this IServiceCollection services) {

            // Register application services (use cases, handlers, validators, etc.)
            //
            // Example (when you create them):
            // services.AddScoped<CreateOrderUseCase>();
            return services;
        }
    }
}
