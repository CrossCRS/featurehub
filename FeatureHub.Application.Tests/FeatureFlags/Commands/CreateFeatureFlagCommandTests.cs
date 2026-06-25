using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using FeatureHub.Application.FeatureFlags.Commands.CreateFeatureFlag;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.FeatureFlags.Commands;

public class CreateFeatureFlagCommandTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();
    private readonly Mock<IProjectAuthorization> _mockProjectAuthorization = new();

    [Fact]
    public async Task CreateFeatureFlag_ShouldAddFeatureFlagToContext()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var environmentsMockDbSet = environments.BuildMockDbSet();
        var featureFlags = new List<FeatureFlag>();
        var featureFlagsMockDbSet = featureFlags.BuildMockDbSet();

        _mockContext.Setup(c => c.Environments).Returns(environmentsMockDbSet.Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlagsMockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new CreateFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new CreateFeatureFlagCommand("owner1", 1, 1, "new-flag", "Description", true, null);

        await handler.HandleAsync(command);

        _mockContext.Verify(c => c.FeatureFlags.Add(It.IsAny<FeatureFlag>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateFeatureFlag_ShouldThrowNotFoundException_WhenEnvironmentDoesNotExist()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>();
        var environmentsMockDbSet = environments.BuildMockDbSet();
        var featureFlags = new List<FeatureFlag>();
        var featureFlagsMockDbSet = featureFlags.BuildMockDbSet();

        _mockContext.Setup(c => c.Environments).Returns(environmentsMockDbSet.Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlagsMockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new CreateFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new CreateFeatureFlagCommand("owner1", 1, 999, "new-flag", "Description", true, null);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task CreateFeatureFlag_ShouldThrowForbiddenAccessException_WhenUserHasNoAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>();
        var environmentsMockDbSet = environments.BuildMockDbSet();
        var featureFlags = new List<FeatureFlag>();
        var featureFlagsMockDbSet = featureFlags.BuildMockDbSet();

        _mockContext.Setup(c => c.Environments).Returns(environmentsMockDbSet.Object);
        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlagsMockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "other-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new CreateFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new CreateFeatureFlagCommand("other-user", 1, 1, "new-flag", "Description", true, null);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.HandleAsync(command));
    }
}
