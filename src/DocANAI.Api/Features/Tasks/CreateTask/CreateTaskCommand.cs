using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.Tasks.CreateTask;

public sealed record CreateTaskCommand(IdOf<User> UserId, Guid ModelId, string PriorityLevel)
    : IRequest<Result<CreateTaskResponse, string>>;
