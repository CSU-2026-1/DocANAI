using System.Text.RegularExpressions;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Context.Options;
using Microsoft.EntityFrameworkCore;

using var cts = new CancellationTokenSource();
var ct = cts.Token;

string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var definiteEnvMigrationNameRegex = new Regex($".+__({envName}).cs");

string? connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

await using var dbContext = new PostgreSqlDbContext(
    DbContextOptionsFactory.CreateOptions<PostgreSqlDbContext>(connectionString));

var pendingMigrationsNames = await dbContext.Database.GetPendingMigrationsAsync(ct);

foreach (string migrationName in pendingMigrationsNames)
{
    if (definiteEnvMigrationNameRegex.IsMatch(migrationName))
    {
        if (migrationName.EndsWith($"__{envName}.cs"))
        {
            await dbContext.Database.MigrateAsync(migrationName, ct);
        }
        continue;
    }

    await dbContext.Database.MigrateAsync(migrationName, ct);
    Console.WriteLine($"Applied migration: {migrationName}");
}

Console.WriteLine("Database migration finished successfully!");