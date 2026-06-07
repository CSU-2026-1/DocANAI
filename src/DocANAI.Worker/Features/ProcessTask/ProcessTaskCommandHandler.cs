using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Repositories.ProcessingTasks;
using DocANAI.Persistence.ValueObjects;
using DocANAI.Worker.Features.BuildReport;
using DocANAI.Worker.Features.ParseDocuments;
using DocANAI.Worker.Features.RunAnalysis;
using MediatR;
using TaskStatusEnum = DocANAI.Persistence.Entities.ProcessingTask.TaskStatus;


namespace DocANAI.Worker.Features.ProcessTask;

internal sealed class ProcessTaskCommandHandler(
    IMediator mediator,
    IProcessingTasksRepository repository,
    ILogger<ProcessTaskCommandHandler> logger)
    : IRequestHandler<ProcessTaskCommand>
{
    public async Task Handle(ProcessTaskCommand request, CancellationToken ct)
    {
        var taskId = request.TaskId;

        try
        {
            var maybeTask = await repository.GetByIdAsync(taskId, ct);
            if (maybeTask.HasNoValue) return;

            var task = maybeTask.Value;
            if (task.Status != TaskStatusEnum.Processing) return;

            logger.LogInformation("Task {TaskId}: Starting document parsing...", (Guid)taskId);
            var documentText = await mediator.Send(new ExtractTextFromDocumentsQuery(taskId), ct);

            logger.LogInformation("Task {TaskId}: Starting LLM analysis...", (Guid)taskId);
            await mediator.Send(new RunAnalysisCommand(taskId, documentText), ct);

            logger.LogInformation("Task {TaskId}: Generating report...", (Guid)taskId);
            await mediator.Send(new BuildExcelReportCommand(taskId), ct);

            task.Complete();
            await repository.UpdateAsync(task, ct);
            logger.LogInformation("Task {TaskId} completed successfully.", (Guid)taskId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Processing failed for task {TaskId}", (Guid)taskId);
            await FailTaskAsync(taskId, ct);
            throw;
        }
    }

    private async Task FailTaskAsync(IdOf<ProcessingTask> taskId, CancellationToken ct)
    {
        var maybeTask = await repository.GetByIdAsync(taskId, ct);
        if (maybeTask.HasNoValue) return;

        var task = maybeTask.Value;
        if (task.Status is TaskStatusEnum.Done or TaskStatusEnum.Failed) return;

        task.Fail();
        await repository.UpdateAsync(task, ct);
    }
}