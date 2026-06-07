using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Worker.Features.ParseDocuments;

public sealed record ExtractTextFromDocumentsQuery(IdOf<ProcessingTask> TaskId)
    : IRequest<string>;