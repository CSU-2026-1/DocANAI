namespace DocANAI.Worker.Infrastructure.Ollama;

public sealed class OllamaSettings
{
    public const string SectionName = "Ollama";

    public string BaseUrl { get; set; } = "http://localhost:11434";

    public int TimeoutSeconds { get; set; } = 120;

    public int MaxRetries { get; set; } = 3;

    public int RetryDelaySeconds { get; set; } = 2;

    public int MaxDocumentCharacters { get; set; } = 120_000;
}
