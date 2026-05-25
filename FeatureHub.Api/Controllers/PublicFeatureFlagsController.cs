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
    public async Task<IActionResult> GetPublicFeatureFlags([FromRoute] string environmentToken, [FromQuery] string? clientHash)
    {
        throw new NotImplementedException();
    }
}
