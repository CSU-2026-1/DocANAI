using CSharpFunctionalExtensions;
using DocANAI.Api.Infrastructure.Authentication;
using DocANAI.Api.Settings;
using DocANAI.Contracts.DTOs.Auth;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.Repositories.RefreshTokens;
using MediatR;
using Microsoft.Extensions.Options;

namespace DocANAI.Api.Features.Auth.Refresh;

internal sealed class RefreshCommandHandler(
    IRefreshTokensRepository refreshTokensRepository,
    IJwtProvider jwtProvider,
    IOptions<JwtSettings> jwtSettings)
    : IRequestHandler<RefreshCommand, Result<AuthResponse, string>>
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<Result<AuthResponse, string>> Handle(RefreshCommand command, CancellationToken ct)
    {
        var maybeStoredToken = await refreshTokensRepository.GetByTokenAsync(command.RefreshToken, ct);
        if (maybeStoredToken.HasNoValue) 
            return Result.Failure<AuthResponse, string>("Invalid or expired refresh token");

        var storedToken = maybeStoredToken.Value;
        
        if (storedToken.IsRevoked || storedToken.ExpiresAt <= DateTime.UtcNow) 
            return Result.Failure<AuthResponse, string>("Invalid or expired refresh token");
        
        storedToken.Revoke(command.IpAddress);
        await refreshTokensRepository.UpdateAsync(storedToken, ct);
        
        var accessToken = jwtProvider.GenerateAccessToken(storedToken.User!);
        var refreshToken = jwtProvider.GenerateRefreshToken();
        
        var refreshTokenEntity = RefreshToken.Create(
            refreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            command.IpAddress,
            storedToken.UserId
        );
        await refreshTokensRepository.AddAsync(refreshTokenEntity, ct);

        return Result.Success<AuthResponse, string>(new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            Username: storedToken.User!.Username,
            UserType: storedToken.User.UserType.ToString()
        ));
    }
}