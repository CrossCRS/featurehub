using FeatureHub.Application.Projects.Commands.UpdateProject;

namespace FeatureHub.Application.Tests.Projects.Commands;

public class UpdateProjectCommandValidatorTests
{
    private readonly UpdateProjectCommandValidator _validator = new();

    [Fact]
    public void UpdateProjectCommandValidator_ShouldHaveValidationError_WhenUserIdIsEmpty()
    {
        var command = new UpdateProjectCommand(string.Empty, 1, "Name", null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateProjectCommand.UserId));
    }

    [Fact]
    public void UpdateProjectCommandValidator_ShouldHaveValidationError_WhenUserIdIsNotGuid()
    {
        var command = new UpdateProjectCommand("not-a-guid", 1, "Name", null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateProjectCommand.UserId));
    }

    [Fact]
    public void UpdateProjectCommandValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var command = new UpdateProjectCommand("deadbeef-0000-0000-0000-000000000042", 0, "Name", null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateProjectCommand.ProjectId));
    }

    [Fact]
    public void UpdateProjectCommandValidator_ShouldHaveValidationError_WhenNameExceedsMaxLength()
    {
        var command = new UpdateProjectCommand("deadbeef-0000-0000-0000-000000000042", 1, new string('a', 101), null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateProjectCommand.Name));
    }

    [Fact]
    public void UpdateProjectCommandValidator_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var command = new UpdateProjectCommand("deadbeef-0000-0000-0000-000000000042", 1, "Valid Name", true);
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
