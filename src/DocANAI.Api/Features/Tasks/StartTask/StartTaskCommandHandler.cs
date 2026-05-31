using CSharpFunctionalExtensions;
using DocANAI.Contracts.Messages;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Repositories.ProcessingTasks;
using DocANAI.Persistence.Repositories.Questions;
using DocANAI.Persistence.Repositories.SourceDocuments;
using MassTransit;
using MediatR;

namespace DocANAI.Api.Features.Tasks.StartTask;

internal sealed class StartTaskCommandHandler(
    IProcessingTasksRepository processingTasksRepository,
    ISourceDocumentsRepository sourceDocumentsRepository,
    IQuestionsRepository questionsRepository,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<StartTaskCommand, Result<StartTaskResponse, string>>
{
    public async Task<Result<StartTaskResponse, string>> Handle(StartTaskCommand command, CancellationToken ct)
    {
        var maybeTask = await processingTasksRepository.GetByIdAsync(command.TaskId, ct);
        if (maybeTask.HasNoValue)
            return Result.Failure<StartTaskResponse, string>("Task not found");

        var task = maybeTask.Value;

        if (task.UserId != command.UserId)
            return Result.Failure<StartTaskResponse, string>("You do not have access to this task");

        if (task.Status != TaskStatus.InQueue)
            return Result.Failure<StartTaskResponse, string>($"Task cannot be started from status '{task.Status}'");

        var sourceDocuments = await sourceDocumentsRepository.GetByTaskIdAsync(command.TaskId, ct);
        if (sourceDocuments.Count == 0)
            return Result.Failure<StartTaskResponse, string>("At least one source document is required");

        var questions = await questionsRepository.GetByTaskIdAsync(command.TaskId, ct);
        if (questions.Count == 0)
            return Result.Failure<StartTaskResponse, string>("At least one question is required");

        task.Start();
        await processingTasksRepository.UpdateAsync(task, ct);

        await publishEndpoint.Publish(new ProcessTaskMessage((Guid)command.TaskId), ct);

        return Result.Success<StartTaskResponse, string>(
            new StartTaskResponse((Guid)command.TaskId, task.Status.ToString()));
    }
}
