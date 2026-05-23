namespace DocANAI.Contracts.DTOs.FileStorage;

public record UploadFileResponse(string FileId, string FileName, long Size);