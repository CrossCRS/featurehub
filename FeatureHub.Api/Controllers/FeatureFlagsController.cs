using FeatureHub.Application.Common.DTOs.FeatureFlag;
using FeatureHub.Application.Common.Interfaces.Identity;
using FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagById;
using FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagsByEnvironment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Darker;

namespace FeatureHub.Api.Controllers;

[Authorize]
[Route("api/Projects/{projectId:int}/Environments/{environmentId:int}/FeatureFlags")]
[ApiController]
public class FeatureFlagsController
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IQueryProcessor _queryProcessor;

    public FeatureFlagsController(ICurrentUserService currentUserService, IQueryProcessor queryProcessor)
    {
        _currentUserService = currentUserService;
        _queryProcessor = queryProcessor;
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
}