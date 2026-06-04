using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagById;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.FeatureFlags.Queries;

public class GetFeatureFlagByIdTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();
    private readonly Mock<IProjectAuthorization> _mockProjectAuthorization = new();

    [Fact]
    public async Task GetFeatureFlagById_ShouldReturnFeatureFlag_WhenFeatureFlagExistsAndUserHasAccess()
    {
        var projects = new List<Project>();
        var environments = new List<Domain.Entities.Environment>();
        var featureFlags = new List<FeatureFlag>();

        projects.AddRange([
            new() { Id = 1, Name = "Project1", OwnerId = "owner1" },
        ]);
        var projectsMockDbSet = projects.BuildMockDbSet();

        environments.AddRange([
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042" },
        ]);
        var environmentsMockDbSet = environments.BuildMockDbSet();

        featureFlags.AddRange([
            new() { Id = 1, EnvironmentId = 1, Name = "FeatureA", Value = true, Environment = environments[0] },
        ]);
        var featureFlagsMockDbSet = featureFlags.BuildMockDbSet();

        _mockContext.Setup(c => c.Projects).Returns(projectsMockDbSet.Object);
        _mockContext.Setup(c => c.Environments).Returns(environmentsMockDbSet.Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlagsMockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new GetFeatureFlagByIdHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetFeatureFlagById("owner1", 1, 1, 1);

        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("FeatureA", result.Name);
    }

    [Fact]
    public async Task GetFeatureFlagById_ShouldThrowNotFoundException_WhenFeatureFlagDoesNotExist()
    {
        var projects = new List<Project>();
        var environments = new List<Domain.Entities.Environment>();
        var featureFlags = new List<FeatureFlag>();

        projects.AddRange([
            new() { Id = 1, Name = "Project1", OwnerId = "owner1" },
        ]);
        var projectsMockDbSet = projects.BuildMockDbSet();

        environments.AddRange([
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042" },
        ]);
        var environmentsMockDbSet = environments.BuildMockDbSet();

        _mockContext.Setup(c => c.Projects).Returns(projectsMockDbSet.Object);
        _mockContext.Setup(c => c.Environments).Returns(environmentsMockDbSet.Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new GetFeatureFlagByIdHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetFeatureFlagById("owner1", 1, 1, 999);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.ExecuteAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetFeatureFlagById_ShouldThrowForbiddenAccessException_WhenUserHasNoAccess()
    {
        var projects = new List<Project>();
        var environments = new List<Domain.Entities.Environment>();
        var featureFlags = new List<FeatureFlag>();

        projects.AddRange([
            new() { Id = 1, Name = "Project1", OwnerId = "owner1" },
        ]);
        var projectsMockDbSet = projects.BuildMockDbSet();

        environments.AddRange([
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042" },
        ]);
        var environmentsMockDbSet = environments.BuildMockDbSet();

        featureFlags.AddRange([
            new() { Id = 1, EnvironmentId = 1, Name = "FeatureA", Value = true, Environment = environments[0] },
        ]);
        var featureFlagsMockDbSet = featureFlags.BuildMockDbSet();

        _mockContext.Setup(c => c.Projects).Returns(projectsMockDbSet.Object);
        _mockContext.Setup(c => c.Environments).Returns(environmentsMockDbSet.Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlagsMockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "other-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new GetFeatureFlagByIdHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetFeatureFlagById("other-user", 1, 1, 1);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.ExecuteAsync(query, CancellationToken.None));
    }
}
