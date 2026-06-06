using CSharpFunctionalExtensions;
using DocANAI.Contracts.DTOs.Tasks;
using DocANAI.Persistence.Repositories.ProcessingTasks;
using DocANAI.Persistence.Repositories.Reports;
using MediatR;

namespace DocANAI.Api.Features.Tasks.GetTaskStatus;

internal sealed class GetTaskStatusCommandHandler(
    IProcessingTasksRepository processingTasksRepository,
    IReportsRepository reportsRepository)
    : IRequestHandler<GetTaskStatusCommand, Result<GetTaskStatusResponse, string>>
{
    public async Task<Result<GetTaskStatusResponse, string>> Handle(GetTaskStatusCommand command, CancellationToken ct)
    {
        var maybeTask = await processingTasksRepository.GetByIdAsync(command.TaskId, ct);
        if (maybeTask.HasNoValue)
            return Result.Failure<GetTaskStatusResponse, string>("Task not found");

        var task = maybeTask.Value;
        if (task.UserId != command.UserId)
            return Result.Failure<GetTaskStatusResponse, string>("Access denied");

        var maybeReport = await reportsRepository.GetByTaskIdAsync(command.TaskId, ct);
        if (maybeReport.HasNoValue)
        {
            return new GetTaskStatusResponse(
                (Guid)command.TaskId,
                string.Empty,
                false,
                task.Status.ToString()
            );
        }

        var report = maybeReport.Value;
        return new GetTaskStatusResponse(
            (Guid)command.TaskId,
            report.FilePath,
            true,
            task.Status.ToString()
        );
    }
}