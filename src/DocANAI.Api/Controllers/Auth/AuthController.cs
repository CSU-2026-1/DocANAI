using System.Security.Claims;
using DocANAI.Api.Services.Auth;
using DocANAI.Contracts.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Controllers.Auth;

/// <summary>
/// Handles authentication and authorization operations (JWT, refresh tokens)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">Authentication service</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="request">Registration data (username, password, optional user type)</param>
    /// <returns>Access and refresh tokens along with user info</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Username already exists or invalid request data</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request, GetIpAddress());
        if (response == null) return BadRequest("Username already exists");
        return Ok(response);
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="request">Login credentials (username and password)</param>
    /// <returns>Access and refresh tokens along with user info</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid username or password</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request, GetIpAddress());
        if (response == null) return Unauthorized("Invalid credentials");
        return Ok(response);
    }

    /// <summary>
    /// Refreshes an expired access token using a valid refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>New access and refresh token pair</returns>
    /// <response code="200">Tokens refreshed successfully</response>
    /// <response code="401">Invalid or expired refresh token</response>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken, GetIpAddress());
        if (response == null) return Unauthorized("Invalid or expired refresh token");
        return Ok(response);
    }

    /// <summary>
    /// Revokes a refresh token (logout)
    /// </summary>
    /// <param name="request">Refresh token to revoke</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">Token revoked successfully</response>
    /// <response code="400">Token not found or already revoked</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke(RevokeTokenRequest request)
    {
        var result = await _authService.RevokeRefreshTokenAsync(request.RefreshToken, GetIpAddress());
        if (!result) return BadRequest("Token not found or already revoked");
        return NoContent();
    }

    /// <summary>
    /// Gets information about the currently authenticated user
    /// </summary>
    /// <returns>User ID, username, and role</returns>
    /// <response code="200">User info retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.Identity?.Name;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        return Ok(new { UserId = userId, Username = username, Role = role });
    }

    private string GetIpAddress() =>
        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}