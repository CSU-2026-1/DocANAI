using DocANAI.Worker.Infrastructure.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRabbitMqMassTransit(builder.Configuration);

var host = builder.Build();
host.Run();
