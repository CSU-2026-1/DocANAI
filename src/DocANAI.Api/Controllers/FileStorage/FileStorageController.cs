using DocANAI.Api.Services.FileStorage;
using DocANAI.Contracts.DTOs;
using DocANAI.Contracts.DTOs.FileStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Controllers.FileStorage;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class FileStorageController : ControllerBase
{
    private readonly IMinioService _minioService;

    public FileStorageController(IMinioService minioService)
    {
        _minioService = minioService;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ErrorResponse("No file uploaded"));
        
        const long maxFileSize = 100 * 1024 * 1024;
        if (file.Length > maxFileSize)
            return BadRequest(new ErrorResponse("File too large", $"Max size: {maxFileSize / 1024 / 1024} MB"));
        
        var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            return BadRequest(new ErrorResponse("Unsupported file type", $"Allowed: {string.Join(", ", allowedExtensions)}"));

        try
        {
            var objectName = await _minioService.UploadFileAsync(file);

            return Ok(new UploadFileResponse(objectName, file.FileName, file.Length));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse("File uploading failed", ex.Message));
        }
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
            return BadRequest(new ErrorResponse("objectName is required"));
        
        if (!IsValidObjectName(objectName))
            return BadRequest(new ErrorResponse("objectName is invalid"));

        try
        {
            var stream = await _minioService.DownloadFileAsync(objectName);
            var fileName = Path.GetFileName(objectName);

            return File(stream, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            return NotFound(new ErrorResponse("File not found", ex.Message));
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
            return BadRequest(new ErrorResponse("objectName is required"));

        if (!IsValidObjectName(objectName))
            return BadRequest(new ErrorResponse("objectName is invalid"));

        try
        {
            var result = await _minioService.DeleteFileAsync(objectName);

            return Ok(new DeleteFileResponse(result, objectName));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse("File deletion failed", ex.Message));
        }
    }

    [HttpGet("presigned-url")]
    public async Task<IActionResult> GetPresignedUrl(string objectName, int expiryMinutes = 5)
    {
        if (string.IsNullOrWhiteSpace(objectName))
            return BadRequest(new ErrorResponse("objectName is required"));
        
        if (!IsValidObjectName(objectName))
            return BadRequest(new ErrorResponse("objectName is invalid"));

        try
        {
            var url = await _minioService.GetPresignedUrlAsync(objectName, expiryMinutes);

            return Ok(new PresignedUrlResponse(url, expiryMinutes * 60));
        }
        catch (Exception ex)
        {
            return NotFound(new ErrorResponse("File not found", ex.Message));
        }
    }

    private static bool IsValidObjectName(string objectName) =>
        !string.IsNullOrWhiteSpace(objectName) &&
        !objectName.Contains("..") &&
        !objectName.Any(c => Path.GetInvalidPathChars().Contains(c));
}