using System.Text;
using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Entities.AIModel;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Repositories.AIModels;
using DocANAI.Persistence.Repositories.Answers;
using DocANAI.Persistence.Repositories.ProcessingTasks;
using DocANAI.Persistence.Repositories.Questions;
using DocANAI.Persistence.Repositories.Reports;
using DocANAI.Persistence.Repositories.SourceDocuments;
using DocANAI.Persistence.ValueObjects;
using DocANAI.Worker.Infrastructure.AI;
using DocANAI.Worker.Infrastructure.Parsing;
using DocANAI.Worker.Infrastructure.Reports;
using DocANAI.Worker.Infrastructure.Storage;

namespace DocANAI.Worker.Features.ProcessTask;

public sealed class ProcessTaskProcessor(
    IProcessingTasksRepository processingTasksRepository,
    ISourceDocumentsRepository sourceDocumentsRepository,
    IQuestionsRepository questionsRepository,
    IAIModelsRepository aiModelsRepository,
    IAnswersRepository answersRepository,
    IReportsRepository reportsRepository,
    IObjectStorageService objectStorageService,
    IDocumentTextExtractor documentTextExtractor,
    IAnswerGenerator answerGenerator,
    IExcelReportBuilder excelReportBuilder,
    ILogger<ProcessTaskProcessor> logger) : IProcessTaskProcessor
{
    private const string ReportExtension = ".xlsx";
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

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

            var maybeModel = await aiModelsRepository.GetByIdAsync(task.ModelId, ct);
            if (maybeModel.HasNoValue)
                throw new InvalidOperationException($"AI model {task.ModelId} not found");

            var modelName = ResolveOllamaModelName(maybeModel.Value);

            var sourceDocuments = await sourceDocumentsRepository.GetByTaskIdAsync(taskIdValue, ct);
            if (sourceDocuments.Count == 0)
                throw new InvalidOperationException("No source documents found for task");

            var questions = await questionsRepository.GetByTaskIdAsync(taskIdValue, ct);
            if (questions.Count == 0)
                throw new InvalidOperationException("No questions found for task");

            var documentText = await ExtractCombinedDocumentTextAsync(sourceDocuments, ct);
            if (string.IsNullOrWhiteSpace(documentText))
                throw new InvalidOperationException("Could not extract text from source documents");

            logger.LogInformation(
                "Task {TaskId}: extracted {CharacterCount} characters from {DocumentCount} document(s)",
                taskId,
                documentText.Length,
                sourceDocuments.Count);

            var reportRows = new List<ReportRow>(questions.Count);
            var answers = new List<Answer>(questions.Count);

            foreach (var question in questions.OrderBy(q => q.QuestionNumber))
            {
                logger.LogInformation(
                    "Task {TaskId}: generating answer for question #{QuestionNumber}",
                    taskId,
                    question.QuestionNumber);

                var answerText = await answerGenerator.GenerateAnswerAsync(
                    modelName,
                    documentText,
                    question,
                    ct);

                answers.Add(Answer.Create(
                    IdOf<Answer>.New(),
                    question.Id,
                    taskIdValue,
                    answerText,
                    modelAccuracy: 0m));

                reportRows.Add(new ReportRow(question.QuestionNumber, question.Text, answerText));
            }

            await answersRepository.AddRangeAsync(answers, ct);
            logger.LogInformation("Task {TaskId}: saved {AnswerCount} answer(s) to database", taskId, answers.Count);

            var reportObjectName = $"reports/{taskId:N}/{Guid.NewGuid()}.xlsx";
            await using (var reportStream = excelReportBuilder.Build(reportRows))
            {
                await objectStorageService.UploadAsync(reportObjectName, reportStream, ExcelContentType, ct);
            }

            logger.LogInformation("Task {TaskId}: report uploaded to {ReportPath}", taskId, reportObjectName);

            var existingReport = await reportsRepository.GetByTaskIdAsync(taskIdValue, ct);
            if (existingReport.HasNoValue)
            {
                var report = Report.Create(
                    IdOf<Report>.New(),
                    taskIdValue,
                    ReportExtension,
                    reportObjectName);

                await reportsRepository.AddAsync(report, ct);
            }

            task.Complete();
            await processingTasksRepository.UpdateAsync(task, ct);

            logger.LogInformation("Task {TaskId} completed successfully", taskId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Processing failed for task {TaskId}", taskId);
            await FailTaskAsync(taskIdValue, ct);
            throw;
        }
    }

    private async Task<string> ExtractCombinedDocumentTextAsync(
        IReadOnlyList<SourceDocument> sourceDocuments,
        CancellationToken ct)
    {
        var builder = new StringBuilder();

        foreach (var document in sourceDocuments)
        {
            await using var stream = await objectStorageService.DownloadAsync(document.FilePath, ct);
            var text = await documentTextExtractor.ExtractAsync(document.Extension, stream, ct);

            builder.AppendLine($"--- Document: {document.Filename} ---");
            builder.AppendLine(text);
            builder.AppendLine();
        }

        return builder.ToString().Trim();
    }

    private static string ResolveOllamaModelName(AIModel model) =>
        string.IsNullOrWhiteSpace(model.Version) ||
        model.Version.Equals("latest", StringComparison.OrdinalIgnoreCase)
            ? model.Name
            : $"{model.Name}:{model.Version}";

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
