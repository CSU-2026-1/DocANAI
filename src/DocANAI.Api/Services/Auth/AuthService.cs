using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DocANAI.Api.Settings;
using DocANAI.Contracts.DTOs.Auth;
using DocANAI.Persistence.Context;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DocANAI.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly PostgreSqlDbContext _dbContext;
    private readonly JwtSettings _jwtSettings;

    public AuthService(PostgreSqlDbContext dbContext, IOptions<JwtSettings> jwtSettings)
    {
        _dbContext = dbContext;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, string ipAddress)
    {
        var exists = await _dbContext.Users.AnyAsync(u => u.Username == request.Username);
        if (exists) return null;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var userId = IdOf<User>.New();
        var userType = request.UserType.HasValue
            ? MapUserType(request.UserType.Value)
            : UserType.Basic;

        var user = User.Create(userId, request.Username, passwordHash, userType);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return await GenerateAuthResponseAsync(user, ipAddress);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, string ipAddress)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null) return null;

        var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!valid) return null;

        return await GenerateAuthResponseAsync(user, ipAddress);
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var storedToken = await _dbContext.Set<RefreshToken>()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
        if (storedToken == null) return null;

        storedToken.Revoke(ipAddress);
        await _dbContext.SaveChangesAsync();

        return await GenerateAuthResponseAsync(storedToken.User!, ipAddress);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var now = DateTime.UtcNow;
        var affected = await _dbContext.Set<RefreshToken>()
            .Where(rt => rt.Token == refreshToken && !rt.IsRevoked)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(rt => rt.IsRevoked, true)
                .SetProperty(rt => rt.RevokedAt, now)
                .SetProperty(rt => rt.RevokedByIp, ipAddress)
            );

        return affected > 0;
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user, string ipAddress)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = RefreshToken.Create(
            refreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            ipAddress,
            user.Id
        );
        _dbContext.Set<RefreshToken>().Add(refreshTokenEntity);
        await _dbContext.SaveChangesAsync();

        return new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            Username: user.Username,
            UserType: user.UserType.ToString()
        );
    }

    private string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.UserType.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static UserType MapUserType(UserTypeDto dto) => dto switch
    {
        UserTypeDto.Basic => UserType.Basic,
        UserTypeDto.Premium => UserType.Premium,
        UserTypeDto.Admin => UserType.Admin,
        _ => UserType.Basic
    };
}