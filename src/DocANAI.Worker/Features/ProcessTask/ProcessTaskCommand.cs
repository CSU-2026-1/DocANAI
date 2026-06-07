using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Worker.Features.ProcessTask;

public sealed record ProcessTaskCommand(IdOf<ProcessingTask> TaskId)
    : IRequest;
