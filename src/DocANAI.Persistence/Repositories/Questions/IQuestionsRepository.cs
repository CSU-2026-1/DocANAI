using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.Questions;

public interface IQuestionsRepository
{
    Task AddRangeAsync(IEnumerable<Question> questions, CancellationToken ct = default);
    Task<List<Question>> GetByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default);
}