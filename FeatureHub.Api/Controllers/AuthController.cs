using FeatureHub.Api.Models.Auth;
using FeatureHub.Application.Auth.Commands.Login;
using FeatureHub.Application.Auth.Commands.RefreshToken;
using Microsoft.AspNetCore.Authorization;
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

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var command = new LoginCommand(loginRequest.Username, loginRequest.Password);
        await _commandProcessor.SendAsync(command);

        return Ok(new { command.AccessToken, command.RefreshToken });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var command = new RefreshTokenCommand(refreshTokenRequest.ExpiredAccessToken, refreshTokenRequest.RefreshToken);
        await _commandProcessor.SendAsync(command);

        return Ok(new { command.NewAccessToken, command.NewRefreshToken });
    }
}
