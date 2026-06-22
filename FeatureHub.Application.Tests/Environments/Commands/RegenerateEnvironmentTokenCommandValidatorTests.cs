using FeatureHub.Application.Environments.Commands.RegenerateEnvironmentToken;

namespace FeatureHub.Application.Tests.Environments.Commands;

public class RegenerateEnvironmentTokenCommandValidatorTests
{
    private readonly RegenerateEnvironmentTokenCommandValidator _validator = new();

    [Fact]
    public void RegenerateEnvironmentTokenCommandValidator_ShouldHaveValidationError_WhenUserIdIsEmpty()
    {
        var command = new RegenerateEnvironmentTokenCommand(string.Empty, 1, 1);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegenerateEnvironmentTokenCommand.UserId));
    }

    [Fact]
    public void RegenerateEnvironmentTokenCommandValidator_ShouldHaveValidationError_WhenUserIdIsNotGuid()
    {
        var command = new RegenerateEnvironmentTokenCommand("not-a-guid", 1, 1);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegenerateEnvironmentTokenCommand.UserId));
    }

    [Fact]
    public void RegenerateEnvironmentTokenCommandValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var command = new RegenerateEnvironmentTokenCommand("deadbeef-0000-0000-0000-000000000042", 0, 1);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegenerateEnvironmentTokenCommand.ProjectId));
    }

    [Fact]
    public void RegenerateEnvironmentTokenCommandValidator_ShouldHaveValidationError_WhenEnvironmentIdIsZero()
    {
        var command = new RegenerateEnvironmentTokenCommand("deadbeef-0000-0000-0000-000000000042", 1, 0);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegenerateEnvironmentTokenCommand.EnvironmentId));
    }

    [Fact]
    public void RegenerateEnvironmentTokenCommandValidator_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var command = new RegenerateEnvironmentTokenCommand("deadbeef-0000-0000-0000-000000000042", 1, 1);
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
