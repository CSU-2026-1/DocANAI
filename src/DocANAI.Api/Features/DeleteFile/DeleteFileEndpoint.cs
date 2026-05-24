using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.DeleteFile;

/// <summary>
/// Endpoint for deleting files from S3 storage.
/// </summary>
/// <param name="mediator">Mediator for sending commands</param>
[Authorize]
[ApiController]
[Route("api/v1/filestorage")]
public sealed class DeleteFileEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Deletes a file from MinIO storage by its unique object name
    /// </summary>
    /// <param name="objectName">Unique path/name of the object in S3 bucket</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Deletion status and file identifier</returns>
    /// <response code="200">File deleted successfully (or already missing)</response>
    /// <response code="400">Invalid object name</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">File deletion failed due to storage error</response>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] string objectName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(objectName) || objectName.Contains(".."))
        {
            return BadRequest(new { Error = "Invalid object name" });
        }

        var command = new DeleteFileCommand(objectName);
        var result = await mediator.Send(command, ct);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : StatusCode(StatusCodes.Status500InternalServerError, new { Error = result.Error });
    }
}