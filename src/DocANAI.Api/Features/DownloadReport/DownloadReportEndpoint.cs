using DocANAI.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.DownloadReport;

/// <summary>
/// Endpoint for downloading report files from S3 storage.
/// </summary>
/// <param name="mediator">Mediator for sending queries</param>
[Authorize]
[ApiController]
[Route("api/v1/filestorage")]
[Tags("FileStorage")]
public sealed class DownloadReportEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Downloads an Excel report file associated with the task
    /// </summary>
    /// <param name="objectName">Unique path/name of the object in S3 bucket</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Excel file stream as an attachment</returns>
    /// <response code="200">File downloaded successfully</response>
    /// <response code="400">Invalid object name</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">File not found in storage</response>
    [HttpGet("download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Download([FromQuery] string objectName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(objectName) || objectName.Contains(".."))
        {
            return BadRequest(new { Error = "Invalid object name" });
        }

        var query = new DownloadReportQuery(objectName);
        var result = await mediator.Send(query, ct);

        if (result.IsFailure)
        {
            return NotFound(new { Error = result.Error });
        }

        var fileName = Path.GetFileName(objectName);
        return File(result.Value, "application/octet-stream", fileName);
    }
}