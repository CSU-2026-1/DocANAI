using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.FileStorage;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.UploadFiles;

public sealed record UploadFilesCommand(IdOf<ProcessingTask> TaskId, IdOf<User> UserId, IFormFile File, bool IsQuestionFile)
    : IRequest<Result<UploadFileResponse, string>>;