using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Projects.Commands.CreateProject;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Projects.Commands;

public class CreateProjectCommandTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();

    [Fact]
    public async Task CreateProject_ShouldAddProjectToContext()
    {
        var projects = new List<Project>();
        var mockDbSet = projects.BuildMockDbSet();
        _mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);

        var handler = new CreateProjectCommandHandler(_mockContext.Object);
        var command = new CreateProjectCommand("deadbeef-0000-0000-0000-000000000042", "New Project");

        await handler.HandleAsync(command);

        _mockContext.Verify(c => c.Projects.Add(It.IsAny<Project>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
