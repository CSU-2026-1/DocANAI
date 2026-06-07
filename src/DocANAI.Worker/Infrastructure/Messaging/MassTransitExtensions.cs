using DocANAI.Worker.Features.ProcessTask;
using MassTransit;

namespace DocANAI.Worker.Infrastructure.Messaging;

public static class MassTransitExtensions
{
    public static IServiceCollection AddRabbitMqMassTransit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var host = configuration["RabbitMQ:Host"] ?? "localhost";
        var port = ushort.TryParse(configuration["RabbitMQ:Port"], out var parsedPort) ? parsedPort : (ushort)5672;
        var username = configuration["RabbitMQ:Username"] ?? "guest";
        var password = configuration["RabbitMQ:Password"] ?? "guest";
        var virtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<ProcessTaskConsumer>();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, port, virtualHost, hostConfigurator =>
                {
                    hostConfigurator.Username(username);
                    hostConfigurator.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
