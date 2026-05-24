using CSharpFunctionalExtensions;
using DocANAI.Api.Infrastructure.Storage;
using DocANAI.Contracts.DTOs.FileStorage;
using MediatR;

namespace DocANAI.Api.Features.GetPresignedUrl;

internal sealed class GetPresignedUrlQueryHandler(IMinioService minioService)
    : IRequestHandler<GetPresignedUrlQuery, Result<PresignedUrlResponse, string>>
{
    public async Task<Result<PresignedUrlResponse, string>> Handle(GetPresignedUrlQuery query, CancellationToken ct)
    {
        try
        {
            var url = await minioService.GetPresignedUrlAsync(query.ObjectName, query.ExpiryMinutes);
            var expiresSeconds = query.ExpiryMinutes * 60;
            
            return Result.Success<PresignedUrlResponse, string>(new PresignedUrlResponse(url, expiresSeconds));
        }
        catch (Exception ex)
        {
            return Result.Failure<PresignedUrlResponse, string>($"Failed to generate presigned URL: {ex.Message}");
        }
    }
}