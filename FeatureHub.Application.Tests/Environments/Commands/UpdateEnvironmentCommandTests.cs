using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using FeatureHub.Application.Environments.Commands.UpdateEnvironment;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Environments.Commands;

public class UpdateEnvironmentCommandTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();
    private readonly Mock<IProjectAuthorization> _mockProjectAuthorization = new();

    [Fact]
    public async Task UpdateEnvironment_ShouldUpdateName_WhenEnvironmentExistsAndUserHasAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Old Name", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new UpdateEnvironmentCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new UpdateEnvironmentCommand("owner1", 1, 1, "New Name", null);

        await handler.HandleAsync(command);

        Assert.Equal("New Name", environments[0].Name);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEnvironment_ShouldUpdateIsActive_WhenEnvironmentExistsAndUserHasAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Environment 1", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new UpdateEnvironmentCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new UpdateEnvironmentCommand("owner1", 1, 1, null, false);

        await handler.HandleAsync(command);

        Assert.False(environments[0].IsActive);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEnvironment_ShouldThrowNotFoundException_WhenEnvironmentDoesNotExist()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>();
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);

        var handler = new UpdateEnvironmentCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new UpdateEnvironmentCommand("owner1", 1, 999, "New Name", null);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task UpdateEnvironment_ShouldThrowForbiddenAccessException_WhenUserHasNoAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Environment 1", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "other-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new UpdateEnvironmentCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new UpdateEnvironmentCommand("other-user", 1, 1, "New Name", null);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.HandleAsync(command));
    }
}
