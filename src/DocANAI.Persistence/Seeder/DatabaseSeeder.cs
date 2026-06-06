using Autofac;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.AIModel;
using DocANAI.Persistence.Entities.Priority;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DocANAI.Persistence.Seeder;

public sealed class DatabaseSeeder : IDatabaseSeeder
{
    private readonly PostgreSqlDbContext _dbContext;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(PostgreSqlDbContext dbContext, ILogger<DatabaseSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(string environment, CancellationToken ct = default)
    {
        _logger.LogInformation("Starting database seeding for environment: {Environment}", environment);
        
        bool formatsChanged = await SeedFormatsAsync(ct);
        bool prioritiesChanged = await SeedPrioritiesAsync(ct);
        bool modelsChanged = await SeedAIModelsAsync(ct);
        bool usersChanged = await SeedUsersAsync(ct);
        
        bool devDataChanged = false;
        
        bool hasChanges = formatsChanged || prioritiesChanged || modelsChanged || usersChanged || devDataChanged;

        if (hasChanges)
        {
            await _dbContext.SaveChangesAsync(ct);
            _logger.LogInformation("Database seeded successfully.");
        }
        else
        {
            _logger.LogInformation("No changes detected. Seeding skipped.");
        }
    }

    private async Task<bool> SeedFormatsAsync(CancellationToken ct)
    {
        if (await _dbContext.Formats.AnyAsync(ct)) return false;
        
        var formats = new[]
        {
            Format.Create(".pdf", 104857600, "application/pdf"),
            Format.Create(".docx", 104857600, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
            Format.Create(".xlsx", 104857600, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        };

        await _dbContext.Formats.AddRangeAsync(formats, ct);
        _logger.LogInformation("Formats seeded successfully.");
        return true;
    }

    private async Task<bool> SeedPrioritiesAsync(CancellationToken ct)
    {
        if (await _dbContext.PriorityLevels.AnyAsync(ct)) return false;

        var normalLevel = PriorityLevel.Create("normal", 1);
        await _dbContext.PriorityLevels.AddAsync(normalLevel, ct);

        var priorityId = IdOf<Priority>.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        var priority = Priority.Create(priorityId, normalLevel);

        await _dbContext.Priorities.AddAsync(priority, ct);
        _logger.LogInformation("Priorities seeded successfully.");
        return true;
    }

    private async Task<bool> SeedAIModelsAsync(CancellationToken ct)
    {
        var targetModelId = IdOf<AIModel>.From(Guid.Parse("99c6eae3-f2f4-400d-a02a-7fc5c76844f3"));
        if (await _dbContext.AIModels.AnyAsync(m => m.Id == targetModelId, ct)) return false;

        var oldModels = await _dbContext.AIModels.ToListAsync(ct);
        if (oldModels.Count > 0)
        {
            _dbContext.AIModels.RemoveRange(oldModels);
            _logger.LogInformation("-> Removing old AI models to make room for llama3.1.");
        }
        
        var defaultModel = AIModel.Create(targetModelId, "llama3.1", "latest");
        
        await _dbContext.AIModels.AddAsync(defaultModel, ct);
        _logger.LogInformation("AI Models seeded successfully.");
        return true;
    }
    
    private async Task<bool> SeedUsersAsync(CancellationToken ct)
    {
        var adminId = IdOf<User>.From(Guid.Empty);
        if (await _dbContext.Users.AnyAsync(u => u.Id == adminId, ct)) return false;
        
        const string secureBcryptHash = "$2a$11$qR3Y5vjP2wO18u.Y9G4vK.m7B9e.vOaE6UoO9uV7Xb0Wd9/g9fK8G";

        var adminUser = User.Create(
            adminId,
            "admin",
            secureBcryptHash,
            UserType.Premium // Значение 1 в вашем enum
        );

        await _dbContext.Users.AddAsync(adminUser, ct);
        _logger.LogInformation("-> Default admin user queued for insert.");
        return true;
    }
}