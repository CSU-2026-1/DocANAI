namespace DocANAI.Contracts.DTOs.Tasks;

public record GetTaskStatusResponse(
    Guid TaskId,
    string ObjectName,
    bool IsReady,
    string? Status
);
