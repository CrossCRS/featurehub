using FeatureHub.Application.Environments.Commands.CreateEnvironment;

namespace FeatureHub.Application.Tests.Environments.Commands;

public class CreateEnvironmentCommandValidatorTests
{
    private readonly CreateEnvironmentCommandValidator _validator = new();

    [Fact]
    public void CreateEnvironmentCommandValidator_ShouldHaveValidationError_WhenNameIsEmpty()
    {
        var command = new CreateEnvironmentCommand("deadbeef-0000-0000-0000-000000000042", 1, string.Empty);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateEnvironmentCommand.Name));
    }

    [Fact]
    public void CreateEnvironmentCommandValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var command = new CreateEnvironmentCommand("deadbeef-0000-0000-0000-000000000042", 0, "Valid Name");
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateEnvironmentCommand.ProjectId));
    }

    [Fact]
    public void CreateEnvironmentCommandValidator_ShouldBeValid_WhenNameAndProjectIdAreProvided()
    {
        var command = new CreateEnvironmentCommand("deadbeef-0000-0000-0000-000000000042", 1, "Valid Environment Name");
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
