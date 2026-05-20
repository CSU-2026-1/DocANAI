using Autofac;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Context.Options;

namespace DocANAI.Persistence;

public sealed class PersistenceInfrastructureModule : Module
{
    public required string? ConnectionString { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        LoadDbContext(builder);
        LoadRepositories(builder);
    }

    private void LoadDbContext(ContainerBuilder builder)
    {
        builder.Register(_ => new PostgreSqlDbContext(
                DbContextOptionsFactory.CreateOptions<PostgreSqlDbContext>(ConnectionString)
            ))
            .AsSelf()
            .InstancePerLifetimeScope();
    }

    private void LoadRepositories(ContainerBuilder builder)
    {
        var repositoriesTypes = GetType().Assembly
            .GetTypes()
            .Where(x => x.GetCustomAttributes(typeof(RepositoryAttribute), false).Length != 0)
            .ToArray();

        builder.RegisterTypes(repositoriesTypes)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}