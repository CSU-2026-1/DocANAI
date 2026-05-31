using System.Text.Json.Serialization;

namespace DocANAI.Worker.Infrastructure.Ollama;

internal sealed class OllamaGenerateRequest
{
    [JsonPropertyName("model")]
    public string Model { get; init; } = null!;

    [JsonPropertyName("prompt")]
    public string Prompt { get; init; } = null!;

    [JsonPropertyName("stream")]
    public bool Stream { get; init; }
}

internal sealed class OllamaGenerateResponse
{
    [JsonPropertyName("response")]
    public string? Response { get; init; }
}
