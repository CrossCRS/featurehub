using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using FeatureHub.Application.Environments.Queries.GetEnvironmentsByProject;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Environments.Queries;

public class GetEnvironmentsByProjectTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();
    private readonly Mock<IProjectAuthorization> _mockProjectAuthorization = new();

    [Fact]
    public async Task GetEnvironmentsByProject_ShouldReturnEnvironments_WhenUserHasAccess()
    {
        var environments = new List<Domain.Entities.Environment>
        {
            new() { Id = 1, Name = "Environment 1", ProjectId = 1, Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
            new() { Id = 2, Name = "Environment 2", ProjectId = 1, Token = "deadbeef000000000000000000000043", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new GetEnvironmentsByProjectHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetEnvironmentsByProject("owner1", 1);

        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetEnvironmentsByProject_ShouldThrowForbiddenAccessException_WhenUserHasNoAccess()
    {
        var environments = new List<Domain.Entities.Environment>
        {
            new() { Id = 1, Name = "Environment 1", ProjectId = 1, Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
            new() { Id = 2, Name = "Environment 2", ProjectId = 1, Token = "deadbeef000000000000000000000043", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "other-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new GetEnvironmentsByProjectHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetEnvironmentsByProject("other-user", 1);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.ExecuteAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetEnvironmentsByProject_ShouldReturnEmptyList_WhenNoEnvironmentsExist()
    {
        var environments = new List<Domain.Entities.Environment>();
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new GetEnvironmentsByProjectHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetEnvironmentsByProject("owner1", 1);

        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
