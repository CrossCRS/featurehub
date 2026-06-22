using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using FeatureHub.Application.Environments.Commands.DeleteEnvironment;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Environments.Commands;

public class DeleteEnvironmentCommandTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();
    private readonly Mock<IProjectAuthorization> _mockProjectAuthorization = new();

    [Fact]
    public async Task DeleteEnvironment_ShouldSetIsDeleted_WhenEnvironmentExistsAndUserHasAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Environment 1", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new DeleteEnvironmentCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new DeleteEnvironmentCommand("owner1", 1, 1);

        await handler.HandleAsync(command);

        Assert.True(environments[0].IsDeleted);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteEnvironment_ShouldThrowNotFoundException_WhenEnvironmentDoesNotExist()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>();
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);

        var handler = new DeleteEnvironmentCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new DeleteEnvironmentCommand("owner1", 1, 999);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task DeleteEnvironment_ShouldThrowForbiddenAccessException_WhenUserHasNoAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Environment 1", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "other-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new DeleteEnvironmentCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new DeleteEnvironmentCommand("other-user", 1, 1);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.HandleAsync(command));
    }
}
