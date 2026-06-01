using DocANAI.Contracts.DTOs;
using DocANAI.Contracts.DTOs.AIModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocANAI.Api.Features.AIModels.GetAIModels;

/// <summary>
/// Returns AI models available for task processing (Ollama).
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/ai-models")]
[Tags("AI Models")]
public sealed class GetAIModelsEndpoint(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lists AI models that can be selected when creating a processing task.
    /// </summary>
    /// <param name="includeUnavailable">Include models marked as unavailable in the database.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<AIModelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] bool includeUnavailable = false, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAIModelsQuery(includeUnavailable), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { Error = result.Error });
    }
}
