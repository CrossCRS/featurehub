using FeatureHub.Api.Models.Projects;
using FeatureHub.Application.Common.Interfaces.Identity;
using FeatureHub.Application.Projects.Commands.CreateProject;
using FeatureHub.Application.Projects.Commands.DeleteProject;
using FeatureHub.Application.Projects.Queries.GetProjectsByOwner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace FeatureHub.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IAmACommandProcessor _commandProcessor;

    public ProjectsController(ICurrentUserService currentUserService, IQueryProcessor queryProcessor, IAmACommandProcessor commandProcessor)
    {
        _currentUserService = currentUserService;
        _queryProcessor = queryProcessor;
        _commandProcessor = commandProcessor;
    }

    [HttpGet]
    public async Task<IEnumerable<ProjectDto>> GetOwnProjects()
    {
        var userId = _currentUserService.GetUserId();
        var result = await _queryProcessor.ExecuteAsync(new GetProjectsByOwner(userId));

        return result;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        var userId = _currentUserService.GetUserId();

        var command = new CreateProjectCommand(userId, request.Name);

        await _commandProcessor.SendAsync(command);

        // TODO: CreatedAt
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProject([FromQuery] int projectId)
    {
        var userId = _currentUserService.GetUserId();

        var command = new DeleteProjectCommand(userId, projectId);

        await _commandProcessor.SendAsync(command);

        return NoContent();
    }
}
