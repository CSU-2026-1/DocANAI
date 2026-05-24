using CSharpFunctionalExtensions;
using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.Reports;

[Repository]
internal sealed class ReportsRepository(PostgreSqlDbContext dbContext) : BaseRepository<Report, IdOf<Report>>(dbContext), IReportsRepository
{
    public async Task<Maybe<Report>> GetByTaskIdAsync(IdOf<ProcessingTask> taskId, CancellationToken ct = default)
    {
        var report = await DbContext.Reports
            .FirstOrDefaultAsync(r => r.TaskId == taskId, ct);

        return report ?? Maybe<Report>.None;
    }
}