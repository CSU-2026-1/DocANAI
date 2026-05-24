using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.Answers;

public interface IAnswersRepository
{
    Task AddRangeAsync(IEnumerable<Answer> answers, CancellationToken ct = default);
    Task<List<Answer>> GetAnswersByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default);
}