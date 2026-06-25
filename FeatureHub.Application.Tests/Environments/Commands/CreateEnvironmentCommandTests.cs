using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using FeatureHub.Application.Environments.Commands.CreateEnvironment;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Environments.Commands;

public class CreateEnvironmentCommandTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();
    private readonly Mock<IProjectAuthorization> _mockProjectAuthorization = new();

    [Fact]
    public async Task CreateEnvironment_ShouldAddEnvironmentToContext()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>();
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new CreateEnvironmentCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new CreateEnvironmentCommand("owner1", 1, "New Environment");

        await handler.HandleAsync(command);

        _mockContext.Verify(c => c.Environments.Add(It.IsAny<FeatureHub.Domain.Entities.Environment>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateEnvironment_ShouldThrowForbiddenAccessException_WhenUserHasNoAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>();
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "other-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new CreateEnvironmentCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new CreateEnvironmentCommand("other-user", 1, "New Environment");

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.HandleAsync(command));
    }
}
