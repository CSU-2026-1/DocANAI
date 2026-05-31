using DocANAI.Api.Settings;
using MassTransit;

namespace DocANAI.Api.Infrastructure.Messaging;

public static class MassTransitExtensions
{
    public static IServiceCollection AddRabbitMqMassTransit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitMqSettings = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>()
            ?? new RabbitMqSettings();

        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.UsingRabbitMq((_, cfg) =>
            {
                cfg.Host(
                    rabbitMqSettings.Host,
                    rabbitMqSettings.Port,
                    rabbitMqSettings.VirtualHost,
                    hostConfigurator =>
                    {
                        hostConfigurator.Username(rabbitMqSettings.Username);
                        hostConfigurator.Password(rabbitMqSettings.Password);
                    });
            });
        });

        return services;
    }
}
