using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.FileStorage;
using MediatR;

namespace DocANAI.Api.Features.GetPresignedUrl;

public sealed record GetPresignedUrlQuery(string ObjectName, int ExpiryMinutes) 
    : IRequest<Result<PresignedUrlResponse, string>>;