using DocANAI.Contracts.DTOs;
using DocANAI.Contracts.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.Auth.Refresh;

/// <summary>
/// Endpoint for token refreshing operations.
/// </summary>
/// <param name="mediator">Mediator for sending commands</param>
[ApiController]
[Route("api/v1/auth")]
[Tags("Auth")]
public sealed class RefreshEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Refreshes an expired access token using a valid refresh token
    /// </summary>
    /// <param name="request">Refresh token data</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>New access and refresh token pair</returns>
    /// <response code="200">Tokens refreshed successfully</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var command = new RefreshCommand(request.RefreshToken, GetIpAddress());
        var result = await mediator.Send(command, ct);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : Unauthorized(new { Error = result.Error });
    }

    private string GetIpAddress() =>
        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}