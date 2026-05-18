using FeatureHub.Api.Models.Environments;
using FeatureHub.Application.Common.DTOs.Environment;
using FeatureHub.Application.Common.Interfaces.Identity;
using FeatureHub.Application.Environments.Commands.CreateEnvironment;
using FeatureHub.Application.Environments.Commands.DeleteEnvironment;
using FeatureHub.Application.Environments.Commands.RegenerateEnvironmentToken;
using FeatureHub.Application.Environments.Commands.UpdateEnvironment;
using FeatureHub.Application.Environments.Queries.GetEnvironmentById;
using FeatureHub.Application.Environments.Queries.GetEnvironmentsByProject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace FeatureHub.Api.Controllers;

[Authorize]
[Route("api/Projects/{projectId:int}/[controller]")]
[ApiController]
public class EnvironmentsController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IAmACommandProcessor _commandProcessor;

    public EnvironmentsController(ICurrentUserService currentUserService, IQueryProcessor queryProcessor, IAmACommandProcessor commandProcessor)
    {
        _currentUserService = currentUserService;
        _queryProcessor = queryProcessor;
        _commandProcessor = commandProcessor;
    }

    [HttpGet]
    public async Task<IEnumerable<EnvironmentDto>> GetEnvironments([FromRoute] int projectId)
    {
        var userId = _currentUserService.GetUserId();
        var result = await _queryProcessor.ExecuteAsync(new GetEnvironmentsByProject(userId, projectId));

        return result;
    }

    [HttpGet("{environmentId:int}")]
    public async Task<EnvironmentDto> GetEnvironmentById([FromRoute] int projectId, [FromRoute] int environmentId)
    {
        var userId = _currentUserService.GetUserId();
        var result = await _queryProcessor.ExecuteAsync(new GetEnvironmentById(userId, projectId, environmentId));

        return result;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEnvironment([FromRoute] int projectId, [FromBody] CreateEnvironmentRequest request)
    {
        var userId = _currentUserService.GetUserId();

        var command = new CreateEnvironmentCommand(userId, projectId, request.Name);

        await _commandProcessor.SendAsync(command);

        return CreatedAtAction(nameof(GetEnvironmentById), new { projectId, environmentId = command.AddedEnvironmentId }, null);
    }

    [HttpPatch("{environmentId:int}")]
    public async Task<IActionResult> UpdateEnvironment([FromRoute] int projectId, [FromRoute] int environmentId, [FromBody] UpdateEnvironmentRequest request)
    {
        var userId = _currentUserService.GetUserId();

        var command = new UpdateEnvironmentCommand(userId, projectId, environmentId, request.Name, request.IsActive);

        await _commandProcessor.SendAsync(command);

        return NoContent();
    }

    [HttpDelete("{environmentId:int}")]
    public async Task<IActionResult> DeleteEnvironment([FromRoute] int projectId, [FromRoute] int environmentId)
    {
        var userId = _currentUserService.GetUserId();

        var command = new DeleteEnvironmentCommand(userId, projectId, environmentId);

        await _commandProcessor.SendAsync(command);

        return NoContent();
    }

    [HttpPost("{environmentId:int}/regenerate-token")]
    public async Task<IActionResult> RegenerateToken([FromRoute] int projectId, [FromRoute] int environmentId)
    {
        var userId = _currentUserService.GetUserId();

        var command = new RegenerateEnvironmentTokenCommand(userId, projectId, environmentId);

        await _commandProcessor.SendAsync(command);

        return Ok(new { token = command.NewToken });
    }
}
