using CSharpFunctionalExtensions;
using DocANAI.Persistence.Repositories.RefreshTokens;
using MediatR;

namespace DocANAI.Api.Features.Revoke;

internal sealed class RevokeCommandHandler(IRefreshTokensRepository refreshTokensRepository)
    : IRequestHandler<RevokeCommand, Result<bool, string>>
{
    public async Task<Result<bool, string>> Handle(RevokeCommand command, CancellationToken ct)
    {
        var maybeStoredToken = await refreshTokensRepository.GetByTokenAsync(command.RefreshToken, ct);
        if (maybeStoredToken.HasNoValue) 
            return Result.Failure<bool, string>("Token not found or already revoked");

        var token = maybeStoredToken.Value;
        
        if (token.IsRevoked) 
            return Result.Failure<bool, string>("Token not found or already revoked");
        
        token.Revoke(command.IpAddress);
        await refreshTokensRepository.UpdateAsync(token, ct);

        return Result.Success<bool, string>(true);
    }
}