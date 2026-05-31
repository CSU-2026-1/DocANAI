namespace DocANAI.Api.Features.Tasks.CreateTask;

public sealed record CreateTaskResponse(
    Guid TaskId,
    Guid ModelId,
    string Status,
    string OllamaModelName);
