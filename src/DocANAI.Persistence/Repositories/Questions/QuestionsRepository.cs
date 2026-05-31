using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.Questions;

[Repository]
internal sealed class QuestionsRepository(PostgreSqlDbContext dbContext) : BaseRepository<Question, IdOf<Question>>(dbContext), IQuestionsRepository
{
    public Task<List<Question>> GetByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default)
        => DbContext.Questions
            .Where(q => q.TaskId == taskId)
            .OrderBy(q => q.QuestionNumber)
            .ToListAsync(ct);
}