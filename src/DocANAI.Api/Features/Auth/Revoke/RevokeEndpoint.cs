using DocANAI.Contracts.DTOs;
using DocANAI.Contracts.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.Auth.Revoke;

/// <summary>
/// Endpoint for revoking refresh tokens (logout).
/// </summary>
/// <param name="mediator">Mediator for sending commands</param>
[Authorize]
[ApiController]
[Route("api/v1/auth")]
[Tags("Auth")]
public sealed class RevokeEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Revokes a refresh token (logout)
    /// </summary>
    /// <param name="request">Refresh token to revoke</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">Token revoked successfully</response>
    /// <response code="400">Token not found or already revoked</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request, CancellationToken ct)
    {
        var command = new RevokeCommand(request.RefreshToken, GetIpAddress());
        var result = await mediator.Send(command, ct);

        return result.IsSuccess 
            ? NoContent() 
            : BadRequest(new { Error = result.Error });
    }

    private string GetIpAddress() =>
        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}