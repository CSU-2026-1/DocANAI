using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Repositories.Answers;
using DocANAI.Persistence.Repositories.Questions;
using DocANAI.Persistence.Repositories.Reports;
using DocANAI.Persistence.ValueObjects;
using DocANAI.Worker.Infrastructure.Reports;
using DocANAI.Worker.Infrastructure.Storage;
using MediatR;

namespace DocANAI.Worker.Features.BuildReport;

public class BuildExcelReportCommandHandler(
    IAnswersRepository answersRepository,
    IQuestionsRepository questionsRepository,
    IReportsRepository reportsRepository,
    IExcelReportBuilder reportBuilder,
    IObjectStorageService storageService)
    : IRequestHandler<BuildExcelReportCommand>
{
    private const string ReportExtension = ".xlsx";
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    
    public async Task Handle(BuildExcelReportCommand request, CancellationToken ct)
    {
        var questions = await questionsRepository.GetByTaskIdAsync(request.TaskId, ct);
        var answers = await answersRepository.GetAnswersByTaskIdAsync(request.TaskId, ct);

        var reportRows = questions
            .OrderBy(q => q.QuestionNumber)
            .Select(q => {
                var ans = answers.FirstOrDefault(a => a.QuestionId == q.Id);
                return new ReportRow(q.QuestionNumber, q.Text, ans?.Text ?? string.Empty);
            })
            .ToList();

        var reportObjectName = $"reports/{request.TaskId:N}/{Guid.NewGuid()}.xlsx";
        await using (var reportStream = reportBuilder.Build(reportRows))
        {
            await storageService.UploadAsync(reportObjectName, reportStream, ExcelContentType, ct);
        }

        var existingReport = await reportsRepository.GetByTaskIdAsync(request.TaskId, ct);
        if (existingReport.HasNoValue)
        {
            var report = Report.Create(
                IdOf<Report>.New(),
                request.TaskId,
                ReportExtension,
                reportObjectName);

            await reportsRepository.AddAsync(report, ct);
        }
        
        
    }
}