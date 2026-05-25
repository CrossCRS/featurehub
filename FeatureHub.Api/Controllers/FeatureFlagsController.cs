using FeatureHub.Api.Models.FeatureFlags;
using FeatureHub.Application.Common.DTOs.FeatureFlag;
using FeatureHub.Application.Common.Interfaces.Identity;
using FeatureHub.Application.FeatureFlags.Commands.CreateFeatureFlag;
using FeatureHub.Application.FeatureFlags.Commands.DeleteFeatureFlag;
using FeatureHub.Application.FeatureFlags.Commands.UpdateFeatureFlag;
using FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagById;
using FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagsByEnvironment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace FeatureHub.Api.Controllers;

[Authorize]
[Route("api/Projects/{projectId:int}/Environments/{environmentId:int}/FeatureFlags")]
[ApiController]
public class FeatureFlagsController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IAmACommandProcessor _commandProcessor;

    public FeatureFlagsController(ICurrentUserService currentUserService, IQueryProcessor queryProcessor, IAmACommandProcessor commandProcessor)
    {
        _currentUserService = currentUserService;
        _queryProcessor = queryProcessor;
        _commandProcessor = commandProcessor;
    }

    [HttpGet]
    public async Task<IEnumerable<FeatureFlagDto>> GetFeatureFlags([FromRoute] int projectId, [FromRoute] int environmentId)
    {
        var userId = _currentUserService.GetUserId();
        var result = await _queryProcessor.ExecuteAsync(new GetFeatureFlagsByEnvironment(userId, projectId, environmentId));

        return result;
    }

    [HttpGet("{featureFlagId:int}")]
    public async Task<FeatureFlagDto> GetFeatureFlagById([FromRoute] int projectId, [FromRoute] int environmentId, [FromRoute] int featureFlagId)
    {
        var userId = _currentUserService.GetUserId();
        var result = await _queryProcessor.ExecuteAsync(new GetFeatureFlagById(userId, projectId, environmentId, featureFlagId));

        return result;
    }

    [HttpPost]
    public async Task<IActionResult> CreateFeatureFlag([FromRoute] int projectId, [FromRoute] int environmentId, [FromBody] CreateFeatureFlagRequest request)
    {
        var userId = _currentUserService.GetUserId();

        var command = new CreateFeatureFlagCommand(userId, projectId, environmentId, request.Name, request.Description, request.Value, request.Data);

        await _commandProcessor.SendAsync(command);

        return CreatedAtAction(nameof(GetFeatureFlagById), new { projectId, environmentId, featureFlagId = command.AddedFeatureFlagId }, null);
    }

    [HttpPatch("{featureFlagId:int}")]
    public async Task<IActionResult> UpdateFeatureFlag([FromRoute] int projectId, [FromRoute] int environmentId, [FromRoute] int featureFlagId, [FromBody] UpdateFeatureFlagRequest request)
    {
        var userId = _currentUserService.GetUserId();

        var command = new UpdateFeatureFlagCommand(userId, projectId, environmentId, featureFlagId, request.Name, request.Description, request.Value, request.Data, request.IsActive);

        await _commandProcessor.SendAsync(command);

        return NoContent();
    }

    [HttpDelete("{featureFlagId:int}")]
    public async Task<IActionResult> DeleteFeatureFlag([FromRoute] int projectId, [FromRoute] int environmentId, [FromRoute] int featureFlagId)
    {
        var userId = _currentUserService.GetUserId();

        var command = new DeleteFeatureFlagCommand(userId, projectId, environmentId, featureFlagId);

        await _commandProcessor.SendAsync(command);

        return NoContent();
    }
}
