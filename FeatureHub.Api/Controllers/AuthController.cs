using FeatureHub.Api.Models.Auth;
using FeatureHub.Application.Auth.Commands.Login;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace FeatureHub.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAmACommandProcessor _commandProcessor;

    public AuthController(IAmACommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var command = new LoginCommand(loginRequest.Username, loginRequest.Password);
        await _commandProcessor.SendAsync(command);

        return Ok(new { command.AccessToken, command.RefreshToken });
    }
}
