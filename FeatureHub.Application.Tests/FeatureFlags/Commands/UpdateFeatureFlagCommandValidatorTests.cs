using FeatureHub.Application.FeatureFlags.Commands.UpdateFeatureFlag;

namespace FeatureHub.Application.Tests.FeatureFlags.Commands;

public class UpdateFeatureFlagCommandValidatorTests
{
    private readonly UpdateFeatureFlagCommandValidator _validator = new();

    [Fact]
    public void UpdateFeatureFlagCommandValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var command = new UpdateFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 0, 1, 1, "flag-name", null, null, null, null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFeatureFlagCommand.ProjectId));
    }

    [Fact]
    public void UpdateFeatureFlagCommandValidator_ShouldHaveValidationError_WhenEnvironmentIdIsZero()
    {
        var command = new UpdateFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 0, 1, "flag-name", null, null, null, null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFeatureFlagCommand.EnvironmentId));
    }

    [Fact]
    public void UpdateFeatureFlagCommandValidator_ShouldHaveValidationError_WhenFeatureFlagIdIsZero()
    {
        var command = new UpdateFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 1, 0, "flag-name", null, null, null, null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFeatureFlagCommand.FeatureFlagId));
    }

    [Fact]
    public void UpdateFeatureFlagCommandValidator_ShouldHaveValidationError_WhenNameContainsInvalidCharacters()
    {
        var command = new UpdateFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 1, 1, "invalid name!", null, null, null, null);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFeatureFlagCommand.Name));
    }

    [Fact]
    public void UpdateFeatureFlagCommandValidator_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var command = new UpdateFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 1, 1, "flag-name", "Description", true, "data", true);
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
