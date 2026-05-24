using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.Priority;

namespace DocANAI.Persistence.Repositories.Priorities;

public interface IPrioritiesRepository
{
    Task<Maybe<Priority>> GetByLevelAsync(string levelName, CancellationToken ct = default);
}