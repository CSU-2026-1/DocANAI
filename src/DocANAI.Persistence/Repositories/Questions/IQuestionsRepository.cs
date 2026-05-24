using DocANAI.Persistence.Entities;

namespace DocANAI.Persistence.Repositories.Questions;

public interface IQuestionsRepository
{
    Task AddRangeAsync(IEnumerable<Question> questions, CancellationToken ct = default);
}