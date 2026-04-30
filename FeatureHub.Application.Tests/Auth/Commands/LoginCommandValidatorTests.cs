using FeatureHub.Application.Auth.Commands.Login;

namespace FeatureHub.Application.Tests.Auth.Commands;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void LoginCommandValidatorTests_ShouldHaveValidationError_WhenUsernameIsEmpty()
    {
        var command = new LoginCommand(string.Empty, "validPassword");
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginCommand.Username));
    }

    [Fact]
    public void LoginCommandValidatorTests_ShouldHaveValidationError_WhenPasswordIsEmpty()
    {
        var command = new LoginCommand("validUser", string.Empty);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginCommand.Password));
    }

    [Fact]
    public void LoginCommandValidatorTests_ShouldHaveValidationError_WhenPasswordIsTooShort()
    {
        var command = new LoginCommand("validUser", "short");
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginCommand.Password));
    }

    [Fact]
    public void LoginCommandValidatorTests_ShouldBeValid_WhenUsernameAndPasswordAreProvided()
    {
        var command = new LoginCommand("validUser", "validPassword");
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
