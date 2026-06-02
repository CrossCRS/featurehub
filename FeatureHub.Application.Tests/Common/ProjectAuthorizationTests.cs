using FeatureHub.Application.Common.Authorization;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Common;

public class ProjectAuthorizationTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();

    [Fact]
    public async Task UserCanAccessProjectAsync_ShouldReturnTrue_WhenUserIsOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1" },
        };
        _mockContext.Setup(c => c.Projects).Returns(projects.BuildMockDbSet().Object);

        var projectAuthorization = new ProjectAuthorization(_mockContext.Object);

        var result = await projectAuthorization.UserCanAccessProjectAsync(1, "owner1");

        Assert.True(result);
    }

    [Fact]
    public async Task UserCanAccessProjectAsync_ShouldReturnFalse_WhenUserIsNotOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1" },
        };
        _mockContext.Setup(c => c.Projects).Returns(projects.BuildMockDbSet().Object);

        var projectAuthorization = new ProjectAuthorization(_mockContext.Object);

        var result = await projectAuthorization.UserCanAccessProjectAsync(1, "other-user");

        Assert.False(result);
    }

    [Fact]
    public async Task UserCanAccessProjectAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
    {
        var projects = new List<Project>();
        _mockContext.Setup(c => c.Projects).Returns(projects.BuildMockDbSet().Object);

        var projectAuthorization = new ProjectAuthorization(_mockContext.Object);

        var result = await projectAuthorization.UserCanAccessProjectAsync(999, "owner1");

        Assert.False(result);
    }

    [Fact]
    public async Task UserCanModifyProjectAsync_ShouldReturnTrue_WhenUserIsOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1" },
        };
        _mockContext.Setup(c => c.Projects).Returns(projects.BuildMockDbSet().Object);

        var projectAuthorization = new ProjectAuthorization(_mockContext.Object);

        var result = await projectAuthorization.UserCanModifyProjectAsync(1, "owner1");

        Assert.True(result);
    }

    [Fact]
    public async Task UserCanModifyProjectAsync_ShouldReturnFalse_WhenUserIsNotOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1" },
        };
        _mockContext.Setup(c => c.Projects).Returns(projects.BuildMockDbSet().Object);

        var projectAuthorization = new ProjectAuthorization(_mockContext.Object);

        var result = await projectAuthorization.UserCanModifyProjectAsync(1, "other-user");

        Assert.False(result);
    }

    [Fact]
    public async Task UserCanModifyProjectAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
    {
        var projects = new List<Project>();
        _mockContext.Setup(c => c.Projects).Returns(projects.BuildMockDbSet().Object);

        var projectAuthorization = new ProjectAuthorization(_mockContext.Object);

        var result = await projectAuthorization.UserCanModifyProjectAsync(999, "owner1");

        Assert.False(result);
    }
}
