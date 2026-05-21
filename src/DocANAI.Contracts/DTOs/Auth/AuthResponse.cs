namespace DocANAI.Contracts.DTOs.Auth;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    string Username,
    string UserType
);