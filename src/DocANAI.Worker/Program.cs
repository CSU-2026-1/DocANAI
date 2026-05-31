using Autofac;
using Autofac.Extensions.DependencyInjection;
using DocANAI.Persistence;
using DocANAI.Worker.Features.ProcessTask;
using DocANAI.Worker.Infrastructure.AI;
using DocANAI.Worker.Infrastructure.Messaging;
using DocANAI.Worker.Infrastructure.Ollama;
using DocANAI.Worker.Infrastructure.Parsing;
using DocANAI.Worker.Infrastructure.Reports;
using DocANAI.Worker.Infrastructure.Storage;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<OllamaSettings>(builder.Configuration.GetSection(OllamaSettings.SectionName));

builder.Services.AddHttpClient<IOllamaClient, OllamaClient>((sp, client) =>
{
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<OllamaSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
});

builder.Services.AddSingleton<IObjectStorageService, MinioObjectStorageService>();
builder.Services.AddSingleton<IDocumentTextExtractor, DocumentTextExtractor>();
builder.Services.AddSingleton<IExcelReportBuilder, ExcelReportBuilder>();
builder.Services.AddScoped<IAnswerGenerator, AnswerGenerator>();
builder.Services.AddScoped<IProcessTaskProcessor, ProcessTaskProcessor>();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule(new PersistenceInfrastructureModule
        {
            ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        });
    });

builder.Services.AddRabbitMqMassTransit(builder.Configuration);

var host = builder.Build();
host.Run();
