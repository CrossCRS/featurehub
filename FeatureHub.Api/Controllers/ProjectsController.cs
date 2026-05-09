using FeatureHub.Application.Common.Interfaces.Identity;
using FeatureHub.Application.Projects.Queries.GetProjectsByOwner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Darker;

namespace FeatureHub.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IQueryProcessor _queryProcessor;

    public ProjectsController(ICurrentUserService currentUserService, IQueryProcessor queryProcessor)
    {
        _currentUserService = currentUserService;
        _queryProcessor = queryProcessor;
    }

    [HttpGet]
    public async Task<IEnumerable<ProjectDto>> GetOwnProjects()
    {
        var userId = _currentUserService.GetUserId();
        var result = await _queryProcessor.ExecuteAsync(new GetProjectsByOwner(userId));

        return result;
    }
}
