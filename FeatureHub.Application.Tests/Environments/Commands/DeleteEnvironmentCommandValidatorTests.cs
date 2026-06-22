using FeatureHub.Application.Environments.Commands.DeleteEnvironment;

namespace FeatureHub.Application.Tests.Environments.Commands;

public class DeleteEnvironmentCommandValidatorTests
{
    private readonly DeleteEnvironmentCommandValidator _validator = new();

    [Fact]
    public void DeleteEnvironmentCommandValidator_ShouldHaveValidationError_WhenUserIdIsEmpty()
    {
        var command = new DeleteEnvironmentCommand(string.Empty, 1, 1);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteEnvironmentCommand.UserId));
    }

    [Fact]
    public void DeleteEnvironmentCommandValidator_ShouldHaveValidationError_WhenUserIdIsNotGuid()
    {
        var command = new DeleteEnvironmentCommand("not-a-guid", 1, 1);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteEnvironmentCommand.UserId));
    }

    [Fact]
    public void DeleteEnvironmentCommandValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var command = new DeleteEnvironmentCommand("deadbeef-0000-0000-0000-000000000042", 0, 1);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteEnvironmentCommand.ProjectId));
    }

    [Fact]
    public void DeleteEnvironmentCommandValidator_ShouldHaveValidationError_WhenEnvironmentIdIsZero()
    {
        var command = new DeleteEnvironmentCommand("deadbeef-0000-0000-0000-000000000042", 1, 0);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteEnvironmentCommand.EnvironmentId));
    }

    [Fact]
    public void DeleteEnvironmentCommandValidator_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var command = new DeleteEnvironmentCommand("deadbeef-0000-0000-0000-000000000042", 1, 1);
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
