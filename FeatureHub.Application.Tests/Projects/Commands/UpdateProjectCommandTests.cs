using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Projects.Commands.UpdateProject;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Projects.Commands;

public class UpdateProjectCommandTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();

    [Fact]
    public async Task UpdateProject_ShouldUpdateName_WhenProjectExistsAndUserIsOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Old Name", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new UpdateProjectCommandHandler(_mockContext.Object);
        var command = new UpdateProjectCommand("owner1", 1, "New Name", null);

        await handler.HandleAsync(command);

        Assert.Equal("New Name", projects[0].Name);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProject_ShouldUpdateIsActive_WhenProjectExistsAndUserIsOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new UpdateProjectCommandHandler(_mockContext.Object);
        var command = new UpdateProjectCommand("owner1", 1, null, false);

        await handler.HandleAsync(command);

        Assert.False(projects[0].IsActive);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProject_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
    {
        var projects = new List<Project>();
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new UpdateProjectCommandHandler(_mockContext.Object);
        var command = new UpdateProjectCommand("owner1", 999, "New Name", null);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task UpdateProject_ShouldThrowForbiddenAccessException_WhenUserIsNotOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new UpdateProjectCommandHandler(_mockContext.Object);
        var command = new UpdateProjectCommand("other-user", 1, "New Name", null);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.HandleAsync(command));
    }
}
