using CSharpFunctionalExtensions;
using DocANAI.Persistence.Entities.AIModel;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Repositories.AIModels;
using DocANAI.Persistence.Repositories.Priorities;
using DocANAI.Persistence.Repositories.ProcessingTasks;
using DocANAI.Persistence.ValueObjects;
using MediatR;

namespace DocANAI.Api.Features.Tasks.CreateTask;

internal sealed class CreateTaskCommandHandler(
    IProcessingTasksRepository processingTasksRepository,
    IAIModelsRepository aiModelsRepository,
    IPrioritiesRepository prioritiesRepository)
    : IRequestHandler<CreateTaskCommand, Result<CreateTaskResponse, string>>
{
    public async Task<Result<CreateTaskResponse, string>> Handle(CreateTaskCommand command, CancellationToken ct)
    {
        var modelId = IdOf<AIModel>.From(command.ModelId);
        var maybeModel = await aiModelsRepository.GetByIdAsync(modelId, ct);

        if (maybeModel.HasNoValue)
            return Result.Failure<CreateTaskResponse, string>("AI model not found");

        var model = maybeModel.Value;
        if (!model.IsAvailable)
            return Result.Failure<CreateTaskResponse, string>("Selected AI model is not available");

        var maybePriority = await prioritiesRepository.GetByLevelAsync(command.PriorityLevel, ct);
        if (maybePriority.HasNoValue)
            return Result.Failure<CreateTaskResponse, string>($"Priority level '{command.PriorityLevel}' not found");

        var taskId = IdOf<ProcessingTask>.New();
        var task = ProcessingTask.Create(
            taskId,
            command.UserId,
            modelId,
            maybePriority.Value.Id);

        await processingTasksRepository.AddAsync(task, ct);

        return Result.Success<CreateTaskResponse, string>(new CreateTaskResponse(
            (Guid)taskId,
            command.ModelId,
            task.Status.ToString(),
            ResolveOllamaModelName(model)));
    }

    private static string ResolveOllamaModelName(AIModel model) =>
        string.IsNullOrWhiteSpace(model.Version) ||
        model.Version.Equals("latest", StringComparison.OrdinalIgnoreCase)
            ? model.Name
            : $"{model.Name}:{model.Version}";
}
