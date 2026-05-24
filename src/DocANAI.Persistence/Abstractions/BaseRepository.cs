using CSharpFunctionalExtensions;
using DocANAI.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Abstractions;

public abstract class BaseRepository<TEntity, TId>(PostgreSqlDbContext dbContext)
    where TEntity : Entity<TId>
    where TId : IComparable<TId>
{
    protected readonly PostgreSqlDbContext DbContext = dbContext;

    public async Task<Maybe<TEntity>> GetByIdAsync(TId id, CancellationToken ct = default)
    {
        var entity = await DbContext.Set<TEntity>()
            .FirstOrDefaultAsync(e => e.Id.Equals(id), ct);

        return entity ?? Maybe<TEntity>.None;
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await DbContext.Set<TEntity>().AddAsync(entity, ct);
        await DbContext.SaveChangesAsync(ct);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        await DbContext.Set<TEntity>().AddRangeAsync(entities, ct);
        await DbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        DbContext.Set<TEntity>().Update(entity);
        await DbContext.SaveChangesAsync(ct);
    }
}