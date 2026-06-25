using FeatureHub.Application.FeatureFlags.Commands.CreateFeatureFlag;

namespace FeatureHub.Application.Tests.FeatureFlags.Commands;

public class CreateFeatureFlagCommandValidatorTests
{
    private readonly CreateFeatureFlagCommandValidator _validator = new();

    [Fact]
    public void CreateFeatureFlagCommandValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var command = new CreateFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 0, 1, "flag-name", null, true, null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateFeatureFlagCommand.ProjectId));
    }

    [Fact]
    public void CreateFeatureFlagCommandValidator_ShouldHaveValidationError_WhenEnvironmentIdIsZero()
    {
        var command = new CreateFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 0, "flag-name", null, true, null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateFeatureFlagCommand.EnvironmentId));
    }

    [Fact]
    public void CreateFeatureFlagCommandValidator_ShouldHaveValidationError_WhenNameIsEmpty()
    {
        var command = new CreateFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 1, string.Empty, null, true, null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateFeatureFlagCommand.Name));
    }

    [Fact]
    public void CreateFeatureFlagCommandValidator_ShouldHaveValidationError_WhenNameContainsInvalidCharacters()
    {
        var command = new CreateFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 1, "invalid name!", null, true, null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateFeatureFlagCommand.Name));
    }

    [Fact]
    public void CreateFeatureFlagCommandValidator_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var command = new CreateFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 1, "flag-name", "Description", true, "data");
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
