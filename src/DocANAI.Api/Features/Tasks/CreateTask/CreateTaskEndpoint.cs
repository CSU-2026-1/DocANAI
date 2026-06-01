using System.Security.Claims;
using DocANAI.Contracts.DTOs;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.Tasks.CreateTask;

[Authorize]
[ApiController]
[Route("api/v1/tasks")]
[Tags("Tasks")]
public sealed class CreateTaskEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Creates a new processing task with the selected AI model.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userGuid))
        {
            return Unauthorized(new { Error = "User is not authorized" });
        }

        if (request.ModelId == Guid.Empty)
            return BadRequest(new { Error = "ModelId is required" });

        var priorityLevel = string.IsNullOrWhiteSpace(request.PriorityLevel)
            ? "normal"
            : request.PriorityLevel.Trim();

        var command = new CreateTaskCommand(
            IdOf<User>.From(userGuid),
            request.ModelId,
            priorityLevel);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { Error = result.Error });
    }
}
