using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.User;

namespace DocANAI.Persistence.Repositories.RefreshTokens;

public interface IRefreshTokensRepository
{
    Task<Maybe<RefreshToken>> GetByTokenAsync(string token, CancellationToken ct = default);
    Task AddAsync(RefreshToken token, CancellationToken ct = default);
    Task UpdateAsync(RefreshToken token, CancellationToken ct = default);
}