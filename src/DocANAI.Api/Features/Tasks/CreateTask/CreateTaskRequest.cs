namespace DocANAI.Api.Features.Tasks.CreateTask;

public sealed record CreateTaskRequest(Guid ModelId, string? PriorityLevel = "normal");
