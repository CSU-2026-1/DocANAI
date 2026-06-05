using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.Tasks;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.Tasks.GetTaskStatus;

public sealed record GetTaskStatusCommand(IdOf<ProcessingTask> TaskId, IdOf<User> UserId)
    : IRequest<Result<GetTaskStatusResponse, string>>;
