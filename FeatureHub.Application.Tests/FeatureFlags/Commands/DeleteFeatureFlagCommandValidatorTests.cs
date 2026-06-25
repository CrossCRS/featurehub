using FeatureHub.Application.FeatureFlags.Commands.DeleteFeatureFlag;

namespace FeatureHub.Application.Tests.FeatureFlags.Commands;

public class DeleteFeatureFlagCommandValidatorTests
{
    private readonly DeleteFeatureFlagCommandValidator _validator = new();

    [Fact]
    public void DeleteFeatureFlagCommandValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var command = new DeleteFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 0, 1, 1);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteFeatureFlagCommand.ProjectId));
    }

    [Fact]
    public void DeleteFeatureFlagCommandValidator_ShouldHaveValidationError_WhenEnvironmentIdIsZero()
    {
        var command = new DeleteFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 0, 1);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteFeatureFlagCommand.EnvironmentId));
    }

    [Fact]
    public void DeleteFeatureFlagCommandValidator_ShouldHaveValidationError_WhenFeatureFlagIdIsZero()
    {
        var command = new DeleteFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 1, 0);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteFeatureFlagCommand.FeatureFlagId));
    }

    [Fact]
    public void DeleteFeatureFlagCommandValidator_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var command = new DeleteFeatureFlagCommand("deadbeef-0000-0000-0000-000000000042", 1, 1, 1);
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
