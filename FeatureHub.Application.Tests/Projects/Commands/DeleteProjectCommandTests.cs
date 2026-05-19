using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Projects.Commands.DeleteProject;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Projects.Commands;

public class DeleteProjectCommandTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();

    [Fact]
    public async Task DeleteProject_ShouldSetIsDeleted_WhenProjectExistsAndUserIsOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new DeleteProjectCommandHandler(_mockContext.Object);
        var command = new DeleteProjectCommand("owner1", 1);

        await handler.HandleAsync(command);

        Assert.True(projects[0].IsDeleted);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProject_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
    {
        var projects = new List<Project>();
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new DeleteProjectCommandHandler(_mockContext.Object);
        var command = new DeleteProjectCommand("owner1", 999);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task DeleteProject_ShouldThrowForbiddenAccessException_WhenUserIsNotOwner()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, OwnerId = "owner1", Name = "Project 1", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new DeleteProjectCommandHandler(_mockContext.Object);
        var command = new DeleteProjectCommand("other-user", 1);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.HandleAsync(command));
    }
}
