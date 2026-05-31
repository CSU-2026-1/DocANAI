using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Repositories.ProcessingTasks;
using DocANAI.Persistence.Repositories.Questions;
using DocANAI.Persistence.Repositories.SourceDocuments;
using DocANAI.Persistence.ValueObjects;
using DocANAI.Worker.Infrastructure.Storage;

namespace DocANAI.Worker.Features.ProcessTask;

public sealed class ProcessTaskProcessor(
    IProcessingTasksRepository processingTasksRepository,
    ISourceDocumentsRepository sourceDocumentsRepository,
    IQuestionsRepository questionsRepository,
    IObjectStorageService objectStorageService,
    ILogger<ProcessTaskProcessor> logger) : IProcessTaskProcessor
{
    public async Task ProcessAsync(Guid taskId, CancellationToken ct = default)
    {
        var taskIdValue = IdOf<ProcessingTask>.From(taskId);

        try
        {
            var maybeTask = await processingTasksRepository.GetByIdAsync(taskIdValue, ct);
            if (maybeTask.HasNoValue)
            {
                logger.LogWarning("Task {TaskId} not found, skipping message", taskId);
                return;
            }

            var task = maybeTask.Value;

            if (task.Status != TaskStatus.Processing)
            {
                logger.LogWarning(
                    "Task {TaskId} has status {Status}, expected Processing. Skipping.",
                    taskId,
                    task.Status);
                return;
            }

            var sourceDocuments = await sourceDocumentsRepository.GetByTaskIdAsync(taskIdValue, ct);
            if (sourceDocuments.Count == 0)
                throw new InvalidOperationException("No source documents found for task");

            var questions = await questionsRepository.GetByTaskIdAsync(taskIdValue, ct);
            if (questions.Count == 0)
                throw new InvalidOperationException("No questions found for task");

            foreach (var document in sourceDocuments)
            {
                await using var stream = await objectStorageService.DownloadAsync(document.FilePath, ct);
                logger.LogInformation(
                    "Downloaded source document {DocumentId} ({Filename}) for task {TaskId}, size: {Size} bytes",
                    document.Id,
                    document.Filename,
                    taskId,
                    stream.Length);
            }

            logger.LogInformation(
                "Task {TaskId} input validated: {DocumentCount} source document(s), {QuestionCount} question(s). Full pipeline pending.",
                taskId,
                sourceDocuments.Count,
                questions.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Processing failed for task {TaskId}", taskId);
            await FailTaskAsync(taskIdValue, ct);
            throw;
        }
    }

    private async Task FailTaskAsync(IdOf<ProcessingTask> taskId, CancellationToken ct)
    {
        var maybeTask = await processingTasksRepository.GetByIdAsync(taskId, ct);
        if (maybeTask.HasNoValue)
            return;

        var task = maybeTask.Value;
        if (task.Status is TaskStatus.Done or TaskStatus.Failed)
            return;

        task.Fail();
        await processingTasksRepository.UpdateAsync(task, ct);

        logger.LogInformation("Task {TaskId} marked as Failed", (Guid)taskId);
    }
}
