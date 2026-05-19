using FeatureHub.Application.Projects.Queries.GetProjectById;

namespace FeatureHub.Application.Tests.Projects.Queries;

public class GetProjectByIdValidatorTests
{
    private readonly GetProjectByIdValidator _validator = new();

    [Fact]
    public void GetProjectByIdValidator_ShouldHaveValidationError_WhenIdIsZero()
    {
        var query = new GetProjectById("deadbeef-0000-0000-0000-000000000042", 0);
        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetProjectById.Id));
    }

    [Fact]
    public void GetProjectByIdValidator_ShouldBeValid_WhenIdIsGreaterThanZero()
    {
        var query = new GetProjectById("deadbeef-0000-0000-0000-000000000042", 1);
        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }
}
