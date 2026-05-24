using CSharpFunctionalExtensions;
using DocANAI.Api.Infrastructure.Storage;
using MediatR;

namespace DocANAI.Api.Features.DownloadReport;

internal sealed class DownloadReportQueryHandler(IMinioService minioService)
    : IRequestHandler<DownloadReportQuery, Result<Stream, string>>
{
    public async Task<Result<Stream, string>> Handle(DownloadReportQuery request, CancellationToken ct)
    {
        try
        {
            var stream = await minioService.DownloadFileAsync(request.ObjectName);
            return Result.Success<Stream, string>(stream);
        }
        catch (Exception ex)
        {
            return Result.Failure<Stream, string>($"Failed to download file: {ex.Message}");
        }
    }
}