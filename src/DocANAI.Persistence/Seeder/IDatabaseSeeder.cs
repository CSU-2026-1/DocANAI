namespace DocANAI.Persistence.Seeder;

public interface IDatabaseSeeder
{
    Task SeedAsync(string environment, CancellationToken ct = default);
}