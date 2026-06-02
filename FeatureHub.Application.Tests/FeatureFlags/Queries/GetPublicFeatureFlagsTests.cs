using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.FeatureFlags.Queries.GetPublicFeatureFlags;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.FeatureFlags.Queries;

public class GetPublicFeatureFlagsTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();

    [Fact]
    public async Task GetPublicFeatureFlags_ShouldReturnFeatureFlags_WhenEnvironmentExists()
    {
        var environments = new List<Domain.Entities.Environment>();
        var featureFlags = new List<FeatureFlag>();

        environments.AddRange([
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", FeatureFlags = featureFlags },
        ]);
        var environmentsMockDbSet = environments.BuildMockDbSet();

        featureFlags.AddRange([
            new() { Id = 1, EnvironmentId = 1, Name = "FeatureA", Value = true, IsActive = true },
            new() { Id = 2, EnvironmentId = 1, Name = "FeatureB", Value = true, IsActive = true },
            new() { Id = 3, EnvironmentId = 1, Name = "FeatureC", Value = true, Data = "Some data", IsActive = true },
        ]);
        var featureFlagsMockDbSet = featureFlags.BuildMockDbSet();

        _mockContext.Setup(c => c.Environments).Returns(environmentsMockDbSet.Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlagsMockDbSet.Object);

        var handler = new GetPublicFeatureFlagsHandler(_mockContext.Object);
        var query = new GetPublicFeatureFlags("deadbeef000000000000000000000042", null);

        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetPublicFeatureFlags_ShouldNotReturnInactiveFeatureFlags_WhenEnvironmentExists()
    {
        var environments = new List<Domain.Entities.Environment>();
        var featureFlags = new List<FeatureFlag>();

        environments.AddRange([
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", FeatureFlags = featureFlags },
        ]);
        var environmentsMockDbSet = environments.BuildMockDbSet();

        featureFlags.AddRange([
            new() { Id = 1, EnvironmentId = 1, Name = "FeatureA", Value = true, IsActive = true },
            new() { Id = 4, EnvironmentId = 1, Name = "FeatureBInactive", Value = true, IsActive = false },
        ]);
        var featureFlagsMockDbSet = featureFlags.BuildMockDbSet();

        _mockContext.Setup(c => c.Environments).Returns(environmentsMockDbSet.Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlagsMockDbSet.Object);

        var handler = new GetPublicFeatureFlagsHandler(_mockContext.Object);
        var query = new GetPublicFeatureFlags("deadbeef000000000000000000000042", null);

        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.DoesNotContain(result, ff => ff.Name == "FeatureBInactive");
    }

    [Fact]
    public async Task GetPublicFeatureFlags_ShouldReturnFalseFeatureFlagsThatHaveData_WhenEnvironmentExists()
    {
        var environments = new List<Domain.Entities.Environment>();
        var featureFlags = new List<FeatureFlag>();

        environments.AddRange([
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", FeatureFlags = featureFlags },
        ]);
        var environmentsMockDbSet = environments.BuildMockDbSet();

        featureFlags.AddRange([
            new() { Id = 1, EnvironmentId = 1, Name = "FeatureA", Value = true, IsActive = true },
            new() { Id = 2, EnvironmentId = 1, Name = "FeatureB", Value = true, IsActive = true },
            new() { Id = 3, EnvironmentId = 1, Name = "FeatureC", Value = false, Data = "Some data", IsActive = true },
        ]);
        var featureFlagsMockDbSet = featureFlags.BuildMockDbSet();

        _mockContext.Setup(c => c.Environments).Returns(environmentsMockDbSet.Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlagsMockDbSet.Object);

        var handler = new GetPublicFeatureFlagsHandler(_mockContext.Object);
        var query = new GetPublicFeatureFlags("deadbeef000000000000000000000042", null);

        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetPublicFeatureFlags_ShouldThrowNotFoundException_WhenEnvironmentDoesNotExist()
    {
        var environments = new List<Domain.Entities.Environment>();
        var environmentsMockDbSet = environments.BuildMockDbSet();

        _mockContext.Setup(c => c.Environments).Returns(environmentsMockDbSet.Object);

        var handler = new GetPublicFeatureFlagsHandler(_mockContext.Object);

        var query = new GetPublicFeatureFlags("nonexistenttoken", null);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.ExecuteAsync(query, CancellationToken.None));
    }
}
