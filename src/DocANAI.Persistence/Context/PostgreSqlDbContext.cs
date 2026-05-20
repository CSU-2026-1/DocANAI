using DocANAI.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Context;

public sealed class PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options) : DbContext(options)
{
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}