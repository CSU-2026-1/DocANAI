using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.Tasks.StartTask;

public sealed record StartTaskCommand(IdOf<ProcessingTask> TaskId, IdOf<User> UserId)
    : IRequest<Result<StartTaskResponse, string>>;
