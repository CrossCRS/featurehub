using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagsByEnvironment;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.FeatureFlags.Queries;

public class GetFeatureFlagsByEnvironmentTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();
    private readonly Mock<IProjectAuthorization> _mockProjectAuthorization = new();

    [Fact]
    public async Task GetFeatureFlagsByEnvironment_ShouldReturnFeatureFlags_WhenUserHasAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var featureFlags = new List<FeatureFlag>
        {
            new() { Id = 1, EnvironmentId = 1, Name = "Flag1", Value = true, IsActive = true, IsDeleted = false, Environment = environments[0] },
            new() { Id = 2, EnvironmentId = 1, Name = "Flag2", Value = false, IsActive = true, IsDeleted = false, Environment = environments[0] },
        };

        _mockContext.Setup(c => c.Environments).Returns(environments.BuildMockDbSet().Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new GetFeatureFlagsByEnvironmentHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetFeatureFlagsByEnvironment("owner1", 1, 1);

        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        var flags = result.ToList();
        Assert.Equal(2, flags.Count);
        Assert.Equal("Flag1", flags[0].Name);
        Assert.Equal("Flag2", flags[1].Name);
    }

    [Fact]
    public async Task GetFeatureFlagsByEnvironment_ShouldThrowForbiddenAccessException_WhenUserHasNoAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var featureFlags = new List<FeatureFlag>
        {
            new() { Id = 1, EnvironmentId = 1, Name = "Flag1", Value = true, IsActive = true, IsDeleted = false, Environment = environments[0] },
        };

        _mockContext.Setup(c => c.Environments).Returns(environments.BuildMockDbSet().Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "other-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new GetFeatureFlagsByEnvironmentHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetFeatureFlagsByEnvironment("other-user", 1, 1);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.ExecuteAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetFeatureFlagsByEnvironment_ShouldReturnEmptyList_WhenNoFeatureFlagsExist()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var featureFlags = new List<FeatureFlag>();

        _mockContext.Setup(c => c.Environments).Returns(environments.BuildMockDbSet().Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new GetFeatureFlagsByEnvironmentHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetFeatureFlagsByEnvironment("owner1", 1, 1);

        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
