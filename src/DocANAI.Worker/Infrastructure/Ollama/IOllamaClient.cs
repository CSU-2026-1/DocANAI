namespace DocANAI.Worker.Infrastructure.Ollama;

public interface IOllamaClient
{
    Task<string> GenerateAsync(string model, string prompt, CancellationToken ct = default);
}
