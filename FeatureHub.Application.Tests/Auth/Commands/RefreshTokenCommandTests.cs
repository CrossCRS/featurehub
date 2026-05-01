using FeatureHub.Application.Auth.Commands.RefreshToken;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces.Identity;
using FeatureHub.Application.Common.Models.Identity;
using Moq;

namespace FeatureHub.Application.Tests.Auth.Commands;

public class RefreshTokenCommandTests
{
    private readonly Mock<IIdentityService> _identityService = new();

    [Fact]
    public async Task RefreshTokenCommandTests_ShouldRefreshToken_WithValidTokens()
    {
        _identityService.Setup(i => i.RefreshTokenAsync("expired-access-token", "valid-refresh-token"))
            .ReturnsAsync(new LoginResponse { AccessToken = "new-valid-token", RefreshToken = "new-valid-refresh-token" });

        var handler = new RefreshTokenCommandHandler(_identityService.Object);
        var command = new RefreshTokenCommand("expired-access-token", "valid-refresh-token");

        var result = await handler.HandleAsync(command);

        Assert.NotNull(result);
        Assert.Equal("new-valid-token", result.NewAccessToken);
        Assert.Equal("new-valid-refresh-token", result.NewRefreshToken);
    }

    [Fact]
    public async Task RefreshTokenCommandTests_ShouldThrowInvalidCredentialsException_WithInvalidTokens()
    {
        _identityService.Setup(i => i.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((LoginResponse?)null);

        var handler = new RefreshTokenCommandHandler(_identityService.Object);
        var command = new RefreshTokenCommand("invalid-expired-access-token", "invalid-refresh-token");

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.HandleAsync(command));
    }
}
