namespace DocANAI.Api.Services.FileStorage;

public interface IMinioService
{
    Task<string> UploadFileAsync(IFormFile file, string? objectName = null);
    Task<Stream> DownloadFileAsync(string objectName);
    Task<bool> DeleteFileAsync(string objectName);
    Task<string> GetPresignedUrlAsync(string objectName, int expiryMinutes = 5);
}