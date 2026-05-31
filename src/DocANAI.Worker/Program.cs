using Autofac;
using Autofac.Extensions.DependencyInjection;
using DocANAI.Persistence;
using DocANAI.Worker.Features.ProcessTask;
using DocANAI.Worker.Infrastructure.Messaging;
using DocANAI.Worker.Infrastructure.Storage;

var builder = Host.CreateApplicationBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule(new PersistenceInfrastructureModule
        {
            ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        });

        containerBuilder.RegisterType<MinioObjectStorageService>()
            .As<IObjectStorageService>()
            .SingleInstance();

        containerBuilder.RegisterType<ProcessTaskProcessor>()
            .As<IProcessTaskProcessor>()
            .InstancePerLifetimeScope();
    });

builder.Services.AddRabbitMqMassTransit(builder.Configuration);

var host = builder.Build();
host.Run();
