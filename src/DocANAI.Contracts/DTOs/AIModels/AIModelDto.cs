namespace DocANAI.Contracts.DTOs.AIModels;

public sealed record AIModelDto(
    Guid Id,
    string Name,
    string Version,
    string OllamaModelName,
    bool IsAvailable);
