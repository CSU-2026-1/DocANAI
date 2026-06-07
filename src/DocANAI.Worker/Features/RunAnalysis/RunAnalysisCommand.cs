using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Worker.Features.RunAnalysis;

public sealed record RunAnalysisCommand(IdOf<ProcessingTask> TaskId, string DocumentText)
    : IRequest; 