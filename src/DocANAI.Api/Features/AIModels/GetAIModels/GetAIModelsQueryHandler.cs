using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.AIModels;
using DocANAI.Persistence.Entities.AIModel;
using DocANAI.Persistence.Repositories.AIModels;
using MediatR;

namespace DocANAI.Api.Features.AIModels.GetAIModels;

internal sealed class GetAIModelsQueryHandler(IAIModelsRepository aiModelsRepository)
    : IRequestHandler<GetAIModelsQuery, Result<IReadOnlyList<AIModelDto>, string>>
{
    public async Task<Result<IReadOnlyList<AIModelDto>, string>> Handle(GetAIModelsQuery request, CancellationToken ct)
    {
        var models = request.IncludeUnavailable
            ? await aiModelsRepository.GetAllModelsAsync(ct)
            : await aiModelsRepository.GetAvailableModelsAsync(ct);

        var dtos = models
            .Select(MapToDto)
            .OrderBy(m => m.Name)
            .ThenBy(m => m.Version)
            .ToList();

        return Result.Success<IReadOnlyList<AIModelDto>, string>(dtos);
    }

    private static AIModelDto MapToDto(AIModel model) =>
        new(
            (Guid)model.Id,
            model.Name,
            model.Version,
            ResolveOllamaModelName(model),
            model.IsAvailable);

    private static string ResolveOllamaModelName(AIModel model) =>
        string.IsNullOrWhiteSpace(model.Version) ||
        model.Version.Equals("latest", StringComparison.OrdinalIgnoreCase)
            ? model.Name
            : $"{model.Name}:{model.Version}";
}
