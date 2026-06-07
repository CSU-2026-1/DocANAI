using CSharpFunctionalExtensions;
using DocANAI.Api.Common.Caching;
using DocANAI.Contracts.DTOs.AIModels;
using MediatR;

namespace DocANAI.Api.Features.AIModels.GetAIModels;

public sealed record GetAIModelsQuery(bool IncludeUnavailable = false)
    : IRequest<Result<IReadOnlyList<AIModelDto>, string>>, ICacheableQuery
{
    public string CacheKey => $"{CacheKeys.AIModels}_{IncludeUnavailable}";
    public TimeSpan Expiration => TimeSpan.FromMinutes(30);
}

