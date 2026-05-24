using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.SourceDocuments;

[Repository]
public class SourceDocumentsRepository(PostgreSqlDbContext dbContext) : BaseRepository<SourceDocument, IdOf<SourceDocument>>(dbContext), ISourceDocumentsRepository
{
    public Task<List<SourceDocument>> GetByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default)
        => DbContext.SourceDocuments
            .Where(d => d.TaskId == taskId)
            .ToListAsync(ct);
}