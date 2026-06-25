using FeatureHub.Application.Environments.Queries.GetEnvironmentById;

namespace FeatureHub.Application.Tests.Environments.Queries;

public class GetEnvironmentByIdValidatorTests
{
    private readonly GetEnvironmentByIdValidator _validator = new();

    [Fact]
    public void GetEnvironmentByIdValidator_ShouldHaveValidationError_WhenEnvironmentIdIsZero()
    {
        var query = new GetEnvironmentById("deadbeef-0000-0000-0000-000000000042", 1, 0);
        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetEnvironmentById.EnvironmentId));
    }

    [Fact]
    public void GetEnvironmentByIdValidator_ShouldBeValid_WhenEnvironmentIdIsGreaterThanZero()
    {
        var query = new GetEnvironmentById("deadbeef-0000-0000-0000-000000000042", 1, 1);
        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }
}
