using CSharpFunctionalExtensions;
using DocANAI.Api.Infrastructure.Authentication;
using DocANAI.Api.Settings;
using DocANAI.Contracts.DTOs.Auth;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.Repositories.RefreshTokens;
using DocANAI.Persistence.Repositories.Users;
using MediatR;
using Microsoft.Extensions.Options;

namespace DocANAI.Api.Features.Auth.Login;

internal sealed class LoginCommandHandler(
    IUsersRepository usersRepository,
    IRefreshTokensRepository refreshTokensRepository,
    IJwtProvider jwtProvider,
    IOptions<JwtSettings> jwtSettings)
    : IRequestHandler<LoginCommand, Result<AuthResponse, string>>
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<Result<AuthResponse, string>> Handle(LoginCommand command, CancellationToken ct)
    {
        var request = command.Request;
        
        var maybeUser = await usersRepository.GetByUsernameAsync(request.Username, ct);
        if (maybeUser.HasNoValue) 
            return Result.Failure<AuthResponse, string>("Invalid credentials");

        var user = maybeUser.Value;
        
        var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!valid) 
            return Result.Failure<AuthResponse, string>("Invalid credentials");
        
        var accessToken = jwtProvider.GenerateAccessToken(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();
        
        var refreshTokenEntity = RefreshToken.Create(
            refreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            command.IpAddress,
            user.Id
        );
        await refreshTokensRepository.AddAsync(refreshTokenEntity, ct);

        return Result.Success<AuthResponse, string>(new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            Username: user.Username,
            UserType: user.UserType.ToString()
        ));
    }
}