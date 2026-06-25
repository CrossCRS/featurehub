using FeatureHub.Application.Environments.Queries.GetEnvironmentsByProject;

namespace FeatureHub.Application.Tests.Environments.Queries;

public class GetEnvironmentsByProjectValidatorTests
{
    private readonly GetEnvironmentsByProjectValidator _validator = new();

    [Fact]
    public void GetEnvironmentsByProjectValidator_ShouldHaveValidationError_WhenProjectIdIsZero()
    {
        var query = new GetEnvironmentsByProject("deadbeef-0000-0000-0000-000000000042", 0);
        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetEnvironmentsByProject.ProjectId));
    }

    [Fact]
    public void GetEnvironmentsByProjectValidator_ShouldBeValid_WhenProjectIdIsGreaterThanZero()
    {
        var query = new GetEnvironmentsByProject("deadbeef-0000-0000-0000-000000000042", 1);
        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }
}
