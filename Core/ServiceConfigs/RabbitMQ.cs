using Core.Services.MessagingService;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.ServiceConfigs;

public static class RabbitMQ
{
    public static IHostApplicationBuilder AddQueueServiceDefaults(this IHostApplicationBuilder builder)
    {
        var rabbitSection = builder.Configuration.GetSection("RabbitMQSetting");
        builder.Services.Configure<RabbitMQSetting>(rabbitSection);
        var rabbitMQSetting = rabbitSection.Get<RabbitMQSetting>();

        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMQSetting.HostName, h =>
                {
                    h.Username(rabbitMQSetting.UserName);
                    h.Password(rabbitMQSetting.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        builder.Services.AddOpenTelemetry().WithTracing(tracing =>
        {
            tracing // Add these lines 👇
                .AddSource("RabbitMQConsumer")
                .AddSource("RabbitMQPublisher");
        });

        return builder;
    }
}