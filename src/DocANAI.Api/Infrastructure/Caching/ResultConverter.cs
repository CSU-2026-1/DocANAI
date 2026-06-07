using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;

namespace DocANAI.Api.Infrastructure.Caching;

public sealed class ResultConverter<T, E> : JsonConverter<Result<T, E>>
{
    public sealed record ResultDto(bool IsSuccess, T? Value, E? Error);
    
    public override Result<T, E> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dto = JsonSerializer.Deserialize<ResultDto>(ref reader, options);
        
        return dto is { IsSuccess: true } 
            ? Result.Success<T, E>(dto.Value!) 
            : Result.Failure<T, E>(dto is null ? default! : dto.Error!);
    }
    
    public override void Write(Utf8JsonWriter writer, Result<T, E> value, JsonSerializerOptions options)
    {
        var dto = new ResultDto(
            value.IsSuccess, 
            value.IsSuccess ? value.Value : default, 
            value.IsFailure ? value.Error : default);
            
        JsonSerializer.Serialize(writer, dto, options);
    }
}