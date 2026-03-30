using System;
using System.Collections.Generic;
using System.Diagnostics;
using EventDrivenArchitecture.RabbitMQ.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenArchitecture.RabbitMQ.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AppApplication(this IServiceCollection services) {

            services.AddScoped<IdempotentUseCaseExecutor>();
            services.AddScoped<ProcessOrderCreatedUseCase>();

            return services;
        }
    }
}
