using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.Answers;

[Repository]
internal sealed class AnswersRepository(PostgreSqlDbContext dbContext) : BaseRepository<Answer, IdOf<Answer>>(dbContext), IAnswersRepository
{
    public Task<List<Answer>> GetAnswersByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default)
        => DbContext.Answers
            .Where(a => a.TaskId == taskId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(ct);
}