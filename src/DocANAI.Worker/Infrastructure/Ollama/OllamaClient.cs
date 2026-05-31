using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace DocANAI.Worker.Infrastructure.Ollama;

public sealed class OllamaClient(
    HttpClient httpClient,
    IOptions<OllamaSettings> options,
    ILogger<OllamaClient> logger) : IOllamaClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly OllamaSettings _settings = options.Value;

    public async Task<string> GenerateAsync(string model, string prompt, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        var payload = new OllamaGenerateRequest
        {
            Model = model,
            Prompt = prompt,
            Stream = false
        };

        var attempt = 0;
        Exception? lastException = null;

        while (attempt < _settings.MaxRetries)
        {
            attempt++;

            try
            {
                using var content = new StringContent(
                    JsonSerializer.Serialize(payload, JsonOptions),
                    Encoding.UTF8,
                    "application/json");

                using var response = await httpClient.PostAsync("api/generate", content, ct).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                    throw new HttpRequestException(
                        $"Ollama returned {(int)response.StatusCode}: {errorBody}",
                        null,
                        response.StatusCode);
                }

                await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
                var result = await JsonSerializer.DeserializeAsync<OllamaGenerateResponse>(stream, JsonOptions, ct)
                    .ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(result?.Response))
                    throw new InvalidOperationException("Ollama returned an empty response");

                return result.Response.Trim();
            }
            catch (Exception ex) when (attempt < _settings.MaxRetries && IsTransient(ex))
            {
                lastException = ex;
                logger.LogWarning(
                    ex,
                    "Ollama request failed (attempt {Attempt}/{MaxRetries}), retrying in {Delay}s",
                    attempt,
                    _settings.MaxRetries,
                    _settings.RetryDelaySeconds);

                await Task.Delay(TimeSpan.FromSeconds(_settings.RetryDelaySeconds), ct).ConfigureAwait(false);
            }
        }

        throw new InvalidOperationException(
            $"Ollama request failed after {_settings.MaxRetries} attempts",
            lastException);
    }

    private static bool IsTransient(Exception ex) =>
        ex is HttpRequestException httpEx && (
            httpEx.StatusCode is null
            or HttpStatusCode.RequestTimeout
            or HttpStatusCode.TooManyRequests
            or >= HttpStatusCode.InternalServerError)
        or ex is TaskCanceledException
        or ex is IOException;
}
