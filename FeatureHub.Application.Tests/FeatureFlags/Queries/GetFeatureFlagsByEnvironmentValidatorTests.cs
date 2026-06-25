using FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagsByEnvironment;

namespace FeatureHub.Application.Tests.FeatureFlags.Queries;

public class GetFeatureFlagsByEnvironmentValidatorTests
{
    private readonly GetFeatureFlagsByEnvironmentValidator _validator = new();

    [Fact]
    public void GetFeatureFlagsByEnvironmentValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var query = new GetFeatureFlagsByEnvironment("deadbeef-0000-0000-0000-000000000042", 0, 1);
        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetFeatureFlagsByEnvironment.ProjectId));
    }

    [Fact]
    public void GetFeatureFlagsByEnvironmentValidator_ShouldHaveValidationError_WhenEnvironmentIdIsZero()
    {
        var query = new GetFeatureFlagsByEnvironment("deadbeef-0000-0000-0000-000000000042", 1, 0);
        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetFeatureFlagsByEnvironment.EnvironmentId));
    }

    [Fact]
    public void GetFeatureFlagsByEnvironmentValidator_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var query = new GetFeatureFlagsByEnvironment("deadbeef-0000-0000-0000-000000000042", 1, 1);
        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }
}
