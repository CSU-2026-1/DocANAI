using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.ProcessingTasks;

public interface IProcessingTasksRepository
{
    Task<Maybe<ProcessingTask>> GetByIdAsync(IdOf<ProcessingTask> id, CancellationToken ct = default);
    Task<List<ProcessingTask>> GetHistoryByUserIdAsync(IdOf<User> userId, CancellationToken ct = default);
    Task AddAsync(ProcessingTask task, CancellationToken ct = default);
    Task UpdateAsync(ProcessingTask task, CancellationToken ct = default);
}