using System.Security.Claims;
using DocANAI.Contracts.DTOs;
using DocANAI.Contracts.DTOs.Auth;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.Auth.Me;

/// <summary>
/// Endpoint for get user info operations.
/// </summary>
/// <param name="mediator">Mediator for sending commands</param>
[ApiController]
[Route("api/v1/auth")]
[Tags("Auth")]
public sealed class MeEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets information about the currently authenticated user
    /// </summary>
    /// <returns>User ID, username, and role</returns>
    /// <response code="200">User info retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Invalid token" });

        var userId = IdOf<User>.From(Guid.Parse(userIdClaim));
        var query = new MeCommand(userId);
        var result = await mediator.Send(query, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { error = result.Error });
    }
}