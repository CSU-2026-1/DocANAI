namespace DocANAI.Worker.Infrastructure.Parsing;

public interface IDocumentTextExtractor
{
    Task<string> ExtractAsync(string extension, Stream content, CancellationToken ct = default);
}
