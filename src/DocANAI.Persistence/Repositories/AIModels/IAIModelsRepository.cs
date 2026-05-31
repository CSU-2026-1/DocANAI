using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.AIModel;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.AIModels;

public interface IAIModelsRepository
{
    Task<Maybe<AIModel>> GetByIdAsync(IdOf<AIModel> id, CancellationToken ct = default);
    Task<List<AIModel>> GetAllModelsAsync(CancellationToken ct = default);
    Task<List<AIModel>> GetAvailableModelsAsync(CancellationToken ct = default);
    Task<Maybe<AIModel>> GetDefaultModelAsync(CancellationToken ct = default);
}