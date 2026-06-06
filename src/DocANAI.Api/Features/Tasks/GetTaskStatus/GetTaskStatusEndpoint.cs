using System.Security.Claims;
using DocANAI.Contracts.DTOs;
using DocANAI.Contracts.DTOs.Tasks;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.Tasks.GetTaskStatus;

[Authorize]
[ApiController]
[Route("api/v1/tasks")]
[Tags("Tasks")]
public sealed class GetTaskStatusEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets information about the status of a task.
    /// </summary>
    /// <param name="taskId">Task identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Report object name and readiness status</returns>
    [HttpGet("{taskId:guid}/status")]
    [ProducesResponseType(typeof(GetTaskStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStatus([FromRoute] Guid taskId, CancellationToken ct)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userGuid))
            return Unauthorized(new ErrorResponse("User is not authorized"));

        var query = new GetTaskStatusCommand(
            IdOf<ProcessingTask>.From(taskId),
            IdOf<User>.From(userGuid));

        var result = await mediator.Send(query, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new ErrorResponse(result.Error));
    }
}
