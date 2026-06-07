using DocANAI.Persistence.Entities;

namespace DocANAI.Worker.Infrastructure.Llm;

public interface IAnswerGenerator
{
    Task<string> GenerateAnswerAsync(
        string modelName,
        string documentText,
        Question question,
        CancellationToken ct = default);
}
