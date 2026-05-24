using CSharpFunctionalExtensions;
using DocANAI.Api.Infrastructure.Storage;
using DocANAI.Contracts.DTOs.FileStorage;
using MediatR;

namespace DocANAI.Api.Features.DeleteFile;

internal sealed class DeleteFileCommandHandler(IMinioService minioService)
    : IRequestHandler<DeleteFileCommand, Result<DeleteFileResponse, string>>
{
    public async Task<Result<DeleteFileResponse, string>> Handle(DeleteFileCommand command, CancellationToken ct)
    {
        try
        {
            var deleted = await minioService.DeleteFileAsync(command.ObjectName);
            return Result.Success<DeleteFileResponse, string>(new DeleteFileResponse(deleted, command.ObjectName));
        }
        catch (Exception ex)
        {
            return Result.Failure<DeleteFileResponse, string>($"Failed to delete file: {ex.Message}");
        }
    }
}