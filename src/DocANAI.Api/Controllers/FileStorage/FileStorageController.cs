using DocANAI.Api.Infrastructure.Storage;
using DocANAI.Contracts.DTOs;
using DocANAI.Contracts.DTOs.FileStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Controllers.FileStorage;

/// <summary>
/// Handles file operations (upload, download, delete, presigned URLs) using MinIO storage
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class FileStorageController : ControllerBase
{
    private readonly IMinioService _minioService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileStorageController"/> class.
    /// </summary>
    /// <param name="minioService">MinIO service for file operations</param>
    public FileStorageController(IMinioService minioService)
    {
        _minioService = minioService;
    }

    /// <summary>
    /// Uploads a file to MinIO storage
    /// </summary>
    /// <param name="file">The file to upload (max 100 MB)</param>
    /// <returns>File identifier, original name, and size</returns>
    /// <response code="200">File uploaded successfully</response>
    /// <response code="400">Invalid file or size exceeded</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost("upload")]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB
    public async Task<ActionResult<UploadFileResponse>> Upload(IFormFile file)
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

    /// <summary>
    /// Downloads a file from MinIO storage
    /// </summary>
    /// <param name="objectName">Unique object identifier (path in bucket)</param>
    /// <returns>File stream as a downloadable attachment</returns>
    /// <response code="200">File downloaded successfully</response>
    /// <response code="400">Invalid objectName</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">File not found</response>
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

    /// <summary>
    /// Deletes a file from MinIO storage
    /// </summary>
    /// <param name="objectName">Unique object identifier (path in bucket)</param>
    /// <returns>Deletion status and file identifier</returns>
    /// <response code="200">File deleted successfully (or already missing)</response>
    /// <response code="400">Invalid objectName</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Deletion failed due to storage error</response>
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

    /// <summary>
    /// Generates a temporary presigned URL for direct file access
    /// </summary>
    /// <param name="objectName">Unique object identifier (path in bucket)</param>
    /// <param name="expiryMinutes">Validity duration in minutes (default 5)</param>
    /// <returns>Presigned URL and expiration time in seconds</returns>
    /// <response code="200">Presigned URL generated</response>
    /// <response code="400">Invalid objectName or expiry value</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">File not found</response>
    [HttpGet("presigned-url")]
    public async Task<ActionResult<PresignedUrlResponse>> GetPresignedUrl(string objectName, int expiryMinutes = 5)
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