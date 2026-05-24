using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;

namespace DocANAI.Persistence.Repositories.Reports;

public interface IReportsRepository
{
    Task AddAsync(Report report, CancellationToken ct = default);
    Task<Maybe<Report>> GetByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default);
}