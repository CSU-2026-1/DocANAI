using System.Security.Claims;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.UploadFiles;

/// <summary>
/// Endpoint for uploading task files to S3 and Database.
/// </summary>
/// <param name="mediator">Mediator for sending commands</param>
[Authorize]
[ApiController]
[Route("api/v1/filestorage")]
public sealed class UploadFilesEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Uploads a file (source document or questions file) and links it to the task in DB
    /// </summary>
    /// <param name="file">The file to upload (max 100 MB)</param>
    /// <param name="taskId">Unique identifier of the task</param>
    /// <param name="isQuestionFile">Flag: true if this is a questions file, false if it's a source doc</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Metadata of the uploaded file</returns>
    /// <response code="200">File uploaded and metadata recorded successfully</response>
    /// <response code="400">Invalid request parameters or upload failure</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost("upload")]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromQuery] Guid taskId,
        [FromQuery] bool isQuestionFile,
        CancellationToken ct)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userGuid))
        {
            return Unauthorized(new { Error = "User is not authorized" });
        }
        
        var command = new UploadFilesCommand(
            IdOf<ProcessingTask>.From(taskId),
            IdOf<User>.From(userGuid),
            file,
            isQuestionFile
        );
        
        var result = await mediator.Send(command, ct);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(new { Error = result.Error });
    }
}