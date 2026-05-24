using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.SourceDocuments;

public interface ISourceDocumentsRepository
{
    Task AddAsync(SourceDocument document, CancellationToken ct = default);
    Task<List<SourceDocument>> GetByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default);
}