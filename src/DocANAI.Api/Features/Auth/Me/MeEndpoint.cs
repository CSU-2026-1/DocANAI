using System.Security.Claims;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.Auth.Me;

[ApiController]
[Route("api/v1/auth")]
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