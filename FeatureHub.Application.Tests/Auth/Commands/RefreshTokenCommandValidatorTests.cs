using FeatureHub.Application.Auth.Commands.RefreshToken;

namespace FeatureHub.Application.Tests.Auth.Commands;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public void RefreshTokenCommandValidatorTests_ShouldHaveValidationError_WhenExpiredTokenIsEmpty()
    {
        var command = new RefreshTokenCommand(string.Empty, "refresh-token");
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RefreshTokenCommand.ExpiredAccessToken));
    }

    [Fact]
    public void RefreshTokenCommandValidatorTests_ShouldHaveValidationError_WhenRefreshTokenIsEmpty()
    {
        var command = new RefreshTokenCommand("expired-access-token", string.Empty);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RefreshTokenCommand.RefreshToken));
    }

    [Fact]
    public void RefreshTokenCommandValidatorTests_ShouldBeValid_WhenExpiredAccessTokenAndRefreshTokenAreProvided()
    {
        var command = new RefreshTokenCommand("expired-access-token", "refresh-token");
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
