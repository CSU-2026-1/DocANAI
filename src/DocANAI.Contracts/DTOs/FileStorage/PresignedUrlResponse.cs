namespace DocANAI.Contracts.DTOs.FileStorage;

public record PresignedUrlResponse(string Url, int ExpiresInSeconds);