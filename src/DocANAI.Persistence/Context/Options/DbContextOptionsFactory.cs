using DocANAI.Persistence.Context.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DocANAI.Persistence.Context.Options;

public static class DbContextOptionsFactory
{
    public static DbContextOptions<TDbContext> CreateOptions<TDbContext>(string? connectionString)
        where TDbContext : DbContext
        => new DbContextOptionsBuilder<TDbContext>()
            .UseNpgsql(connectionString)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .AddInterceptors(new UpdateAuditsSaveChangesInterceptor())
            .UseLoggerFactory(LoggerFactory.Create(x => x.AddConsole()))
            .Options;
}