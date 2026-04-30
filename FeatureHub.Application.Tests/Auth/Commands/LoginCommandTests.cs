using FeatureHub.Application.Auth.Commands.Login;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces.Identity;
using Moq;

namespace FeatureHub.Application.Tests.Auth.Commands;

public class LoginCommandTests
{
    private readonly Mock<IIdentityService> _identityService = new();

    [Fact]
    public async Task LoginCommandTests_ShouldLogin_WithCorrectCredentials()
    {
        _identityService.Setup(i => i.LoginAsync("validUser", "validPassword"))
            .ReturnsAsync("validToken");

        var handler = new LoginCommandHandler(_identityService.Object);
        var command = new LoginCommand("validUser", "validPassword");

        var result = await handler.HandleAsync(command);

        Assert.NotNull(result);
        Assert.Equal("validToken", result.Token);
    }

    [Fact]
    public async Task LoginCommandTests_ShouldThrowInvalidCredentialsException_WithInvalidCredentials()
    {
        _identityService.Setup(i => i.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((string?)null);

        var handler = new LoginCommandHandler(_identityService.Object);
        var command = new LoginCommand("invalidUser", "invalidPassword");

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.HandleAsync(command));
    }
}
