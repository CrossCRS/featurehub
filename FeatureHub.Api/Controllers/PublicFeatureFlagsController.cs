using FeatureHub.Application.Common.DTOs.FeatureFlag;
using FeatureHub.Application.FeatureFlags.Queries.GetPublicFeatureFlags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Darker;

namespace FeatureHub.Api.Controllers;

[AllowAnonymous]
[Route("api/public")]
[ApiController]
public class PublicFeatureFlagsController : ControllerBase
{
    private readonly IQueryProcessor _queryProcessor;

    public PublicFeatureFlagsController(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet("{environmentToken}")]
    public async Task<IEnumerable<PublicFeatureFlagDto>> GetPublicFeatureFlags([FromRoute] string environmentToken, [FromQuery] string? clientHash)
    {
        var query = new GetPublicFeatureFlags(environmentToken, clientHash);
        var featureFlags = await _queryProcessor.ExecuteAsync(query);

        return featureFlags;
    }
}
