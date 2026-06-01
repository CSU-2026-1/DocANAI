using System.Security.Claims;
using DocANAI.Contracts.DTOs;
using DocANAI.Persistence.Entities.ProcessingTask;
using DocANAI.Persistence.Entities.User;
using DocANAI.Persistence.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.Questions.AddManualQuestions;

[Authorize]
[ApiController]
[Route("api/v1/tasks")]
[Tags("Tasks")]
public sealed class AddManualQuestionsEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Adds manually written questions to an existing processing task.
    /// </summary>
    [HttpPost("{taskId:guid}/questions/manual")]
    [ProducesResponseType(typeof(AddManualQuestionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddManualQuestions(
        [FromRoute] Guid taskId,
        [FromBody] AddManualQuestionsRequest request,
        CancellationToken ct)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userGuid))
        {
            return Unauthorized(new { Error = "User is not authorized" });
        }

        if (request.Questions is null)
            return BadRequest(new { Error = "Questions payload is required" });

        var command = new AddManualQuestionsCommand(
            IdOf<ProcessingTask>.From(taskId),
            IdOf<User>.From(userGuid),
            request.Questions);

        var result = await mediator.Send(command, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { Error = result.Error });
    }
}
