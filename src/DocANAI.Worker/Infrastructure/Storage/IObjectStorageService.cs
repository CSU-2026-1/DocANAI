namespace DocANAI.Worker.Infrastructure.Storage;

public interface IObjectStorageService
{
    Task<Stream> DownloadAsync(string objectName, CancellationToken ct = default);

    Task<string> UploadAsync(
        string objectName,
        Stream content,
        string contentType,
        CancellationToken ct = default);
}
