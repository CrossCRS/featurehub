using FeatureHub.Application.FeatureFlags.Queries.GetPublicFeatureFlags;

namespace FeatureHub.Application.Tests.FeatureFlags.Queries;

public class GetPublicFeatureFlagsValidatorTests
{
    private readonly GetPublicFeatureFlagsValidator _validator = new();

    [Fact]
    public void GetPublicFeatureFlagsValidator_ShouldHaveValidationError_WhenEnvironmentTokenIsEmpty()
    {
        var query = new GetPublicFeatureFlags(string.Empty, null);
        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetPublicFeatureFlags.EnvironmentToken));
    }

    [Fact]
    public void GetPublicFeatureFlagsValidator_ShouldBeValid_WhenEnvironmentTokenIsProvided()
    {
        var query = new GetPublicFeatureFlags("deadbeef000000000000000000000042", null);
        var result = _validator.Validate(query);
        Assert.True(result.IsValid);
    }
}
