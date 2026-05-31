using System.Text;
using UglyToad.PdfPig;

namespace DocANAI.Worker.Infrastructure.Parsing;

internal static class PdfDocumentTextExtractor
{
    public static Task<string> ExtractAsync(Stream content, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        using var document = PdfDocument.Open(content);
        var builder = new StringBuilder();

        foreach (var page in document.GetPages())
        {
            ct.ThrowIfCancellationRequested();
            builder.AppendLine(page.Text);
        }

        return Task.FromResult(builder.ToString().Trim());
    }
}
