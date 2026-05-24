using CSharpFunctionalExtensions;
using DocANAI.Api.Infrastructure.Storage;
using DocANAI.Contracts.DTOs.FileStorage;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Repositories.QuestionFiles;
using DocANAI.Persistence.Repositories.SourceDocuments;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.UploadFiles;

internal sealed class UploadFilesCommandHandler(
    IMinioService minioService,
    ISourceDocumentsRepository sourceDocumentsRepository,
    IQuestionFilesRepository questionFilesRepository)
    : IRequestHandler<UploadFilesCommand, Result<UploadFileResponse, string>>
{
    public async Task<Result<UploadFileResponse, string>> Handle(UploadFilesCommand command, CancellationToken ct)
    {
        try
        {
            var objectName = await minioService.UploadFileAsync(command.File, null);

            var extension = Path.GetExtension(command.File.FileName).ToLowerInvariant();
            
            if (command.IsQuestionFile)
            {
                var questionFileId = IdOf<QuestionFile>.New();
                var questionFile = QuestionFile.Create(
                    questionFileId,
                    command.UserId,
                    command.TaskId,
                    extension,
                    command.File.FileName
                );
                
                await questionFilesRepository.AddAsync(questionFile, ct);
            }
            else
            {
                var sourceDocumentId = IdOf<SourceDocument>.New();
                var sourceDocument = SourceDocument.Create(
                    sourceDocumentId,
                    command.UserId,
                    command.TaskId,
                    extension,
                    command.File.FileName,
                    (int)command.File.Length
                );
                
                await sourceDocumentsRepository.AddAsync(sourceDocument, ct);
            }
            
            return Result.Success<UploadFileResponse, string>(new UploadFileResponse(
                FileId: objectName,
                FileName: command.File.FileName,
                Size: command.File.Length
            ));
        }
        catch (Exception ex)
        {
            return Result.Failure<UploadFileResponse, string>($"File upload failed: {ex.Message}");
        }
    }
}