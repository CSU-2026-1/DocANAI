using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.AIModels;
using MediatR;

namespace DocANAI.Api.Features.AIModels.GetAIModels;

public sealed record GetAIModelsQuery(bool IncludeUnavailable = false)
    : IRequest<Result<IReadOnlyList<AIModelDto>, string>>;
