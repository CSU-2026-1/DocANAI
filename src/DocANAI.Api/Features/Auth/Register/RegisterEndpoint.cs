using DocANAI.Contracts.DTOs;
using DocANAI.Contracts.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.Auth.Register;

/// <summary>
/// Endpoint for user registration operations.
/// </summary>
/// <param name="mediator">Mediator for sending commands</param>
[ApiController]
[Route("api/v1/auth")]
[Tags("Auth")]
public sealed class RegisterEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <param name="request">Registration data (username, password, optional user type)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Access and refresh tokens along with user info</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Username already exists or invalid request data</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var command = new RegisterCommand(request, GetIpAddress());
        var result = await mediator.Send(command, ct);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(new { Error = result.Error });
    }
    
    private string GetIpAddress() => HttpContext.Connection.RemoteIpAddress?.ToString() ??  "Unknown";
}