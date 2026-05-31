using CSharpFunctionalExtensions;
using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities.AIModel;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.AIModels;

[Repository]
public class AIModelsRepository(PostgreSqlDbContext dbContext): BaseRepository<AIModel, IdOf<AIModel>>(dbContext), IAIModelsRepository
{
    public Task<List<AIModel>> GetAllModelsAsync(CancellationToken ct = default)
        => DbContext.AIModels.ToListAsync(ct);

    public Task<List<AIModel>> GetAvailableModelsAsync(CancellationToken ct = default)
        => DbContext.AIModels
            .Where(m => m.IsAvailable)
            .ToListAsync(ct);

    public async Task<Maybe<AIModel>> GetDefaultModelAsync(CancellationToken ct = default)
    {
        var model = await DbContext.AIModels
            .FirstOrDefaultAsync(m => m.IsAvailable, ct);

        return model ?? Maybe<AIModel>.None;
    }
}