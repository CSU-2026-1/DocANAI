using Autofac;
using Autofac.Extensions.DependencyInjection;
using DocANAI.Persistence;
using DocANAI.Worker.Features.ProcessTask;
using DocANAI.Worker.Infrastructure.Llm;
using DocANAI.Worker.Infrastructure.Messaging;
using DocANAI.Worker.Infrastructure.Ollama;
using DocANAI.Worker.Infrastructure.Parsing;
using DocANAI.Worker.Infrastructure.Reports;
using DocANAI.Worker.Infrastructure.Storage;
using Microsoft.Extensions.Options;

var builder = Host.CreateDefaultBuilder(args);

builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.ConfigureServices((context, services) =>
{
    services.Configure<OllamaSettings>(context.Configuration.GetSection(OllamaSettings.SectionName));
    services.AddHttpClient<IOllamaClient, OllamaClient>((sp, client) =>
    {
        var settings = sp.GetRequiredService<IOptions<OllamaSettings>>().Value;
        client.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/");
        client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
    });
    services.AddSingleton<IObjectStorageService, MinioObjectStorageService>();
    services.AddSingleton<IDocumentTextExtractor, DocumentTextExtractor>();
    services.AddSingleton<IExcelReportBuilder, ExcelReportBuilder>();
    services.AddScoped<IAnswerGenerator, AnswerGenerator>();
    
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
    
    services.AddRabbitMqMassTransit(context.Configuration);
});

builder.ConfigureContainer<ContainerBuilder>((context, containerBuilder) =>
{
    var connectionString = context.Configuration
        .GetConnectionString("DefaultConnection");
    containerBuilder.RegisterModule(new PersistenceInfrastructureModule
    {
        ConnectionString = connectionString
    });
});

var host = builder.Build();
await host.RunAsync();
