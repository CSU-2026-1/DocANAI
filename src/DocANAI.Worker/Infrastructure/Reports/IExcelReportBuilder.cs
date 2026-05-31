namespace DocANAI.Worker.Infrastructure.Reports;

public interface IExcelReportBuilder
{
    MemoryStream Build(IReadOnlyList<ReportRow> rows);
}
