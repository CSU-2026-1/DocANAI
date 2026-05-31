using ClosedXML.Excel;

namespace DocANAI.Worker.Infrastructure.Reports;

public sealed class ExcelReportBuilder : IExcelReportBuilder
{
    public MemoryStream Build(IReadOnlyList<ReportRow> rows)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");

        worksheet.Cell(1, 1).Value = "№";
        worksheet.Cell(1, 2).Value = "Question";
        worksheet.Cell(1, 3).Value = "Answer";

        var headerRange = worksheet.Range(1, 1, 1, 3);
        headerRange.Style.Font.Bold = true;

        var rowIndex = 2;
        foreach (var row in rows.OrderBy(r => r.QuestionNumber))
        {
            worksheet.Cell(rowIndex, 1).Value = row.QuestionNumber;
            worksheet.Cell(rowIndex, 2).Value = row.QuestionText;
            worksheet.Cell(rowIndex, 3).Value = row.AnswerText;
            rowIndex++;
        }

        worksheet.Columns().AdjustToContents();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }
}
