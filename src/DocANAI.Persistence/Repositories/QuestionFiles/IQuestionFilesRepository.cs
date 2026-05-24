using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.QuestionFiles;

public interface IQuestionFilesRepository
{
    Task AddAsync(QuestionFile file, CancellationToken ct = default);
    Task<Maybe<QuestionFile>> GetByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default);
}