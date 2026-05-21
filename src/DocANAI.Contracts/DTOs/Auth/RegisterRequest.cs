namespace DocANAI.Contracts.DTOs.Auth;

public record RegisterRequest(
    string Username,
    string Password,
    UserTypeDto? UserType = null
);