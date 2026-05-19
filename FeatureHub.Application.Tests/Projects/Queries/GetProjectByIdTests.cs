using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Projects.Queries.GetProjectById;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Projects.Queries;

public class GetProjectByIdTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();

    [Fact]
    public async Task GetProjectById_ShouldReturnProject_WhenProjectExistsAndUserIsOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new GetProjectByIdHandler(_mockContext.Object);
        var query = new GetProjectById("owner1", 1);

        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("owner1", result.OwnerId);
        Assert.Equal("Project 1", result.Name);
    }

    [Fact]
    public async Task GetProjectById_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
    {
        var projects = new List<Project>();
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new GetProjectByIdHandler(_mockContext.Object);
        var query = new GetProjectById("owner1", 999);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.ExecuteAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetProjectById_ShouldThrowForbiddenAccessException_WhenUserIsNotOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new GetProjectByIdHandler(_mockContext.Object);
        var query = new GetProjectById("other-user", 1);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.ExecuteAsync(query, CancellationToken.None));
    }
}
