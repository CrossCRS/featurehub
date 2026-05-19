using FeatureHub.Application.Projects.Commands.DeleteProject;

namespace FeatureHub.Application.Tests.Projects.Commands;

public class DeleteProjectCommandValidatorTests
{
    private readonly DeleteProjectCommandValidator _validator = new();

    [Fact]
    public void DeleteProjectCommandValidator_ShouldHaveValidationError_WhenUserIdIsEmpty()
    {
        var command = new DeleteProjectCommand(string.Empty, 1);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteProjectCommand.UserId));
    }

    [Fact]
    public void DeleteProjectCommandValidator_ShouldHaveValidationError_WhenUserIdIsNotGuid()
    {
        var command = new DeleteProjectCommand("not-a-guid", 1);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteProjectCommand.UserId));
    }

    [Fact]
    public void DeleteProjectCommandValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var command = new DeleteProjectCommand("deadbeef-0000-0000-0000-000000000042", 0);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteProjectCommand.ProjectId));
    }

    [Fact]
    public void DeleteProjectCommandValidator_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var command = new DeleteProjectCommand("deadbeef-0000-0000-0000-000000000042", 1);
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
