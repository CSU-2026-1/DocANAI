using CSharpFunctionalExtensions;
using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.Attributes;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DocANAI.Persistence.Repositories.RefreshTokens;

[Repository]
internal sealed class RefreshTokensRepository(PostgreSqlDbContext dbContext) : BaseRepository<RefreshToken, IdOf<RefreshToken>>(dbContext), IRefreshTokensRepository
{
    public async Task<Maybe<RefreshToken>> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        var refreshToken = await DbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, ct);

        return refreshToken ?? Maybe<RefreshToken>.None;
    }
}