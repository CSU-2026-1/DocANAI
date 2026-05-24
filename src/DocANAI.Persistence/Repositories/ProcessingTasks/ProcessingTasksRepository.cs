using CSharpFunctionalExtensions;
using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.ProcessingTasks;

[Repository]
internal sealed class ProcessingTasksRepository(PostgreSqlDbContext dbContext) : BaseRepository<ProcessingTask, IdOf<ProcessingTask>>(dbContext), IProcessingTasksRepository
{
    public Task<List<ProcessingTask>> GetHistoryByUserIdAsync(IdOf<User> userId, CancellationToken ct = default)
        => DbContext.ProcessingTasks
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
}