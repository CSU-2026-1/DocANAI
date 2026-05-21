using DocANAI.Contracts.DTOs.Auth;

namespace DocANAI.Api.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request, string ipAddress);
    Task<AuthResponse?> LoginAsync(LoginRequest request, string ipAddress);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken, string ipAddress);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, string ipAddress);
}