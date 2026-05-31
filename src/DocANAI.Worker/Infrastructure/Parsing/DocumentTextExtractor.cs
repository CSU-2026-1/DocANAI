namespace DocANAI.Worker.Infrastructure.Parsing;

public sealed class DocumentTextExtractor : IDocumentTextExtractor
{
    public Task<string> ExtractAsync(string extension, Stream content, CancellationToken ct = default)
    {
        var normalized = NormalizeExtension(extension);

        return normalized switch
        {
            "pdf" => PdfDocumentTextExtractor.ExtractAsync(content, ct),
            "docx" => DocxDocumentTextExtractor.ExtractAsync(content, ct),
            _ => throw new NotSupportedException($"Document format '{extension}' is not supported. Supported: .pdf, .docx")
        };
    }

    private static string NormalizeExtension(string extension) =>
        extension.Trim().TrimStart('.').ToLowerInvariant();
}
