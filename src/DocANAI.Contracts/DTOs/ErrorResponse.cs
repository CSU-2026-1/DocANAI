namespace DocANAI.Contracts.DTOs;

public record ErrorResponse(string Error, string? Details = null);