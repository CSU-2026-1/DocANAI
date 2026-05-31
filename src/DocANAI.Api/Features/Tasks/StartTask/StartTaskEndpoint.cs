using System.Security.Claims;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.Tasks.StartTask;

[Authorize]
[ApiController]
[Route("api/v1/tasks")]
public sealed class StartTaskEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Starts task processing: updates status and publishes a message to RabbitMQ.
    /// </summary>
    [HttpPost("{taskId:guid}/start")]
    public async Task<IActionResult> Start([FromRoute] Guid taskId, CancellationToken ct)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userGuid))
        {
            return Unauthorized(new { Error = "User is not authorized" });
        }

        var command = new StartTaskCommand(
            IdOf<ProcessingTask>.From(taskId),
            IdOf<User>.From(userGuid));

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { Error = result.Error });
    }
}
