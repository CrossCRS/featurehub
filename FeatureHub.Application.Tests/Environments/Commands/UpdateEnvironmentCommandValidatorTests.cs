using FeatureHub.Application.Environments.Commands.UpdateEnvironment;

namespace FeatureHub.Application.Tests.Environments.Commands;

public class UpdateEnvironmentCommandValidatorTests
{
    private readonly UpdateEnvironmentCommandValidator _validator = new();

    [Fact]
    public void UpdateEnvironmentCommandValidator_ShouldHaveValidationError_WhenUserIdIsEmpty()
    {
        var command = new UpdateEnvironmentCommand(string.Empty, 1, 1, "Name", null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateEnvironmentCommand.UserId));
    }

    [Fact]
    public void UpdateEnvironmentCommandValidator_ShouldHaveValidationError_WhenUserIdIsNotGuid()
    {
        var command = new UpdateEnvironmentCommand("not-a-guid", 1, 1, "Name", null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateEnvironmentCommand.UserId));
    }

    [Fact]
    public void UpdateEnvironmentCommandValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var command = new UpdateEnvironmentCommand("deadbeef-0000-0000-0000-000000000042", 0, 1, "Name", null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateEnvironmentCommand.ProjectId));
    }

    [Fact]
    public void UpdateEnvironmentCommandValidator_ShouldHaveValidationError_WhenEnvironmentIdIsZero()
    {
        var command = new UpdateEnvironmentCommand("deadbeef-0000-0000-0000-000000000042", 1, 0, "Name", null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateEnvironmentCommand.EnvironmentId));
    }

    [Fact]
    public void UpdateEnvironmentCommandValidator_ShouldHaveValidationError_WhenNameExceedsMaxLength()
    {
        var command = new UpdateEnvironmentCommand("deadbeef-0000-0000-0000-000000000042", 1, 1, new string('a', 101), null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateEnvironmentCommand.Name));
    }

    [Fact]
    public void UpdateEnvironmentCommandValidator_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var command = new UpdateEnvironmentCommand("deadbeef-0000-0000-0000-000000000042", 1, 1, "Valid Name", true);
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
