using Microsoft.AspNetCore.Mvc;

namespace FeatureHub.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Hello!");
    }
}
