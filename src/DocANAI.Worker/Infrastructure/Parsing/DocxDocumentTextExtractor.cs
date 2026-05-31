using DocumentFormat.OpenXml.Packaging;

namespace DocANAI.Worker.Infrastructure.Parsing;

internal static class DocxDocumentTextExtractor
{
    public static Task<string> ExtractAsync(Stream content, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        using var document = WordprocessingDocument.Open(content, false);
        var body = document.MainDocumentPart?.Document?.Body;
        var text = body?.InnerText ?? string.Empty;

        return Task.FromResult(text.Trim());
    }
}
