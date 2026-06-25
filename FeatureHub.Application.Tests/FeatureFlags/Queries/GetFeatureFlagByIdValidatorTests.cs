using FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagById;

namespace FeatureHub.Application.Tests.FeatureFlags.Queries;

public class GetFeatureFlagByIdValidatorTests
{
    private readonly GetFeatureFlagByIdValidator _validator = new();

    [Fact]
    public void GetFeatureFlagByIdValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var query = new GetFeatureFlagById("deadbeef-0000-0000-0000-000000000042", 0, 1, 1);
        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetFeatureFlagById.ProjectId));
    }

    [Fact]
    public void GetFeatureFlagByIdValidator_ShouldHaveValidationError_WhenEnvironmentIdIsZero()
    {
        var query = new GetFeatureFlagById("deadbeef-0000-0000-0000-000000000042", 1, 0, 1);
        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetFeatureFlagById.EnvironmentId));
    }

    [Fact]
    public void GetFeatureFlagByIdValidator_ShouldHaveValidationError_WhenFeatureFlagIdIsZero()
    {
        var query = new GetFeatureFlagById("deadbeef-0000-0000-0000-000000000042", 1, 1, 0);
        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetFeatureFlagById.FeatureFlagId));
    }

    [Fact]
    public void GetFeatureFlagByIdValidator_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        var query = new GetFeatureFlagById("deadbeef-0000-0000-0000-000000000042", 1, 1, 1);
        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }
}
