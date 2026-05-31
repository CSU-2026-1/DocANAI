using System.Text;
using DocANAI.Persistence.Entities;
using DocANAI.Worker.Infrastructure.Ollama;
using Microsoft.Extensions.Options;

namespace DocANAI.Worker.Infrastructure.AI;

public sealed class AnswerGenerator(IOllamaClient ollamaClient, IOptions<OllamaSettings> options) : IAnswerGenerator
{
    private readonly OllamaSettings _settings = options.Value;

    public Task<string> GenerateAnswerAsync(
        string modelName,
        string documentText,
        Question question,
        CancellationToken ct = default)
    {
        var truncatedDocument = TruncateDocument(documentText);
        var prompt = BuildPrompt(truncatedDocument, question);
        return ollamaClient.GenerateAsync(modelName, prompt, ct);
    }

    private string TruncateDocument(string documentText)
    {
        if (documentText.Length <= _settings.MaxDocumentCharacters)
            return documentText;

        return documentText[.._settings.MaxDocumentCharacters] +
               "\n\n[Document truncated due to length limits.]";
    }

    private static string BuildPrompt(string documentText, Question question)
    {
        var builder = new StringBuilder();
        builder.AppendLine("You are a document analysis assistant.");
        builder.AppendLine("Answer ONLY using the document content below.");
        builder.AppendLine("If the answer is not present in the document, reply exactly:");
        builder.AppendLine("\"The document does not contain enough information.\"");
        builder.AppendLine();
        builder.AppendLine("DOCUMENT:");
        builder.AppendLine(documentText);
        builder.AppendLine();
        builder.AppendLine($"QUESTION #{question.QuestionNumber}:");
        builder.AppendLine(question.Text);
        builder.AppendLine();
        builder.AppendLine("ANSWER:");
        return builder.ToString();
    }
}
