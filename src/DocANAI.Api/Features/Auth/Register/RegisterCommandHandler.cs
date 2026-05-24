using CSharpFunctionalExtensions;
using DocANAI.Api.Infrastructure.Authentication;
using DocANAI.Api.Settings;
using DocANAI.Contracts.DTOs.Auth;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.Repositories.RefreshTokens;
using DocANAI.Persistence.Repositories.Users;
using DocANAI.Persistence.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;

namespace DocANAI.Api.Features.Auth.Register;

internal sealed class RegisterCommandHandler(
    IUsersRepository usersRepository,
    IRefreshTokensRepository refreshTokensRepository,
    IJwtProvider jwtProvider,
    IOptions<JwtSettings> jwtSettings)
    : IRequestHandler<RegisterCommand, Result<AuthResponse, string>>
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    
    public async Task<Result<AuthResponse, string>> Handle(RegisterCommand command, CancellationToken ct)
    {
        var request = command.Request;

        var maybeUser = await usersRepository.GetByUsernameAsync(request.Username, ct);
        if (maybeUser.HasValue) 
            return Result.Failure<AuthResponse, string>("Username already exists");

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var userId = IdOf<User>.New();

        var userType = request.UserType.HasValue 
            ? (UserType)request.UserType.Value 
            : UserType.Basic;

        var user = User.Create(userId, request.Username, passwordHash, userType);
        await usersRepository.AddAsync(user, ct);

        
        
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