using CSharpFunctionalExtensions;
using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.QuestionFiles;

[Repository]
public class QuestionFilesRepository(PostgreSqlDbContext dbContext) : BaseRepository<QuestionFile, IdOf<QuestionFile>>(dbContext), IQuestionFilesRepository
{
    public async Task<Maybe<QuestionFile>> GetByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default)
    {
        var file = await DbContext.QuestionFiles
            .FirstOrDefaultAsync(f => f.TaskId == taskId, ct);

        return file ?? Maybe<QuestionFile>.None;
    }
}