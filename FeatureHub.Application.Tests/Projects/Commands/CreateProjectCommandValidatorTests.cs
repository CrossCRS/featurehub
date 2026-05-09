using FeatureHub.Application.Projects.Commands.CreateProject;

namespace FeatureHub.Application.Tests.Projects.Commands;

public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator = new();

    [Fact]
    public void CreateProjectCommandValidator_ShouldHaveValidationError_WhenNameIsEmpty()
    {
        var command = new CreateProjectCommand("deadbeef-0000-0000-0000-000000000042", string.Empty);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProjectCommand.Name));
    }

    [Fact]
    public void CreateProjectCommandValidator_ShouldHaveValidationError_WhenOwnerIdIsEmpty()
    {
        var command = new CreateProjectCommand(string.Empty, "Valid Project Name");
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProjectCommand.OwnerId));
    }

    [Fact]
    public void CreateProjectCommandValidator_ShouldHaveValidationError_WhenOwnerIdIsNotGuid()
    {
        var command = new CreateProjectCommand("not-a-guid", "Valid Project Name");
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProjectCommand.OwnerId));
    }

    [Fact]
    public void CreateProjectCommandValidator_ShouldBeValid_WhenNameAndOwnerIdAreProvided()
    {
        var command = new CreateProjectCommand("deadbeef-0000-0000-0000-000000000042", "Valid Project Name");
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
