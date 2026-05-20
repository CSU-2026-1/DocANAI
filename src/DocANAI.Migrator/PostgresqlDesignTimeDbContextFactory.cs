using DocANAI.Persistence.Context;
using DocANAI.Persistence.Context.Options;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DocANAI.Migrator;

public sealed class PostgresqlDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PostgreSqlDbContext>
{
    public PostgreSqlDbContext CreateDbContext(string[] args)
    {
        string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        
        return new PostgreSqlDbContext(
            DbContextOptionsFactory.CreateOptions<PostgreSqlDbContext>(connectionString));
    }
}