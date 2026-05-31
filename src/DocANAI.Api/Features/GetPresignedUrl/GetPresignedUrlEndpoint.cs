using DocANAI.Contracts.DTOs;
using DocANAI.Contracts.DTOs.FileStorage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.GetPresignedUrl;

/// <summary>
/// Endpoint for generating temporary presigned URLs.
/// </summary>
/// <param name="mediator">Mediator for sending queries</param>
[Authorize]
[ApiController]
[Route("api/v1/filestorage")]
[Tags("FileStorage")]
public sealed class GetPresignedUrlEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Generates a temporary presigned URL for direct file access
    /// </summary>
    /// <param name="objectName">Unique path/name of the object in S3 bucket</param>
    /// <param name="expiryMinutes">Validity duration in minutes (default 5)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Presigned URL and expiration time in seconds</returns>
    /// <response code="200">Presigned URL generated successfully</response>
    /// <response code="400">Invalid object name or expiry value</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">File not found in storage</response>
    [HttpGet("presigned-url")]
    [ProducesResponseType(typeof(PresignedUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPresignedUrl(
        [FromQuery] string objectName,
        [FromQuery] int expiryMinutes = 5,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(objectName) || objectName.Contains(".."))
        {
            return BadRequest(new { Error = "Invalid object name" });
        }

        if (expiryMinutes <= 0)
        {
            return BadRequest(new { Error = "Expiry minutes must be greater than zero" });
        }

        var query = new GetPresignedUrlQuery(objectName, expiryMinutes);
        var result = await mediator.Send(query, ct);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : NotFound(new { Error = result.Error });
    }
}