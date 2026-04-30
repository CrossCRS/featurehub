using FeatureHub.Application.Features.Hello.Queries;
using Microsoft.AspNetCore.Mvc;
using Paramore.Darker;

namespace FeatureHub.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    private readonly IQueryProcessor _queryProcessor;

    public HelloController(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var query = new GetHelloQuery();
        var result = await _queryProcessor.ExecuteAsync(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
