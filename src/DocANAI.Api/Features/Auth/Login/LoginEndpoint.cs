using DocANAI.Contracts.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.Auth.Login;

/// <summary>
/// Endpoint for user login operations.
/// </summary>
/// <param name="mediator">Mediator for sending commands</param>
[ApiController]
[Route("api/v1/auth")]
public sealed class LoginEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="request">Login credentials (username and password)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Access and refresh tokens along with user info</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid username or password</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var command = new LoginCommand(request, GetIpAddress());
        var result = await mediator.Send(command, ct);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : Unauthorized(new { result.Error });
    }

    private string GetIpAddress() =>
        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}