using CSharpFunctionalExtensions;
using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities.Priority;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.Priorities;

[Repository]
public class PrioritiesRepository(PostgreSqlDbContext dbContext) :  BaseRepository<Priority, IdOf<Priority>>(dbContext), IPrioritiesRepository
{
    public async Task<Maybe<Priority>> GetByLevelAsync(string levelName, CancellationToken ct = default)
    {
        var priority = await DbContext.Priorities
            .Include(p => p.PriorityLevel)
            .FirstOrDefaultAsync(p => p.PriorityLevel.Id == levelName, ct);

        return priority ?? Maybe<Priority>.None;
    }
}