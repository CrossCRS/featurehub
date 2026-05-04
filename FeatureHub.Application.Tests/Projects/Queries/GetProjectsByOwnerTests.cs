using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Projects.Queries.GetProjectsByOwner;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Projects.Queries;

public class GetProjectsByOwnerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();

    [Fact]
    public async Task GetProjectsByOwner_ShouldReturnProjects_ForValidOwnerId()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1", IsActive = true, IsDeleted = false },
            new() { Id = 2, OwnerId = "owner2", Name = "Project 2", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new GetProjectsByOwnerHandler(_mockContext.Object);
        var ownerId = "owner1";

        var result = await handler.ExecuteAsync(new GetProjectsByOwner(ownerId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.All(result, p => Assert.Equal(ownerId, p.OwnerId));
    }

    [Fact]
    public async Task GetProjectsByOwner_ShouldReturnEmptyList_ForInvalidOwnerId()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1", IsActive = true, IsDeleted = false },
            new() { Id = 2, OwnerId = "owner2", Name = "Project 2", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new GetProjectsByOwnerHandler(_mockContext.Object);
        var ownerId = "nonexistent-owner";

        var result = await handler.ExecuteAsync(new GetProjectsByOwner(ownerId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProjectsByOwner_ShouldNotReturnDeletedProjects()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1", IsActive = true, IsDeleted = false },
            new() { Id = 2, OwnerId = "owner1", Name = "Project 2", IsActive = true, IsDeleted = true },
        };
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new GetProjectsByOwnerHandler(_mockContext.Object);
        var ownerId = "owner1";

        var result = await handler.ExecuteAsync(new GetProjectsByOwner(ownerId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }
}
