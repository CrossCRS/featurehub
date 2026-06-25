using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using FeatureHub.Application.FeatureFlags.Commands.DeleteFeatureFlag;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.FeatureFlags.Commands;

public class DeleteFeatureFlagCommandTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();
    private readonly Mock<IProjectAuthorization> _mockProjectAuthorization = new();

    [Fact]
    public async Task DeleteFeatureFlag_ShouldSetIsDeleted_WhenFeatureFlagExistsAndUserHasAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var featureFlags = new List<FeatureFlag>
        {
            new() { Id = 1, EnvironmentId = 1, Name = "Flag1", Value = true, IsActive = true, IsDeleted = false, Environment = environments[0] },
        };

        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new DeleteFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new DeleteFeatureFlagCommand("owner1", 1, 1, 1);

        await handler.HandleAsync(command);

        Assert.True(featureFlags[0].IsDeleted);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteFeatureFlag_ShouldThrowNotFoundException_WhenFeatureFlagDoesNotExist()
    {
        var featureFlags = new List<FeatureFlag>();

        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);

        var handler = new DeleteFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new DeleteFeatureFlagCommand("owner1", 1, 1, 999);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task DeleteFeatureFlag_ShouldThrowForbiddenAccessException_WhenUserHasNoAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var featureFlags = new List<FeatureFlag>
        {
            new() { Id = 1, EnvironmentId = 1, Name = "Flag1", Value = true, IsActive = true, IsDeleted = false, Environment = environments[0] },
        };

        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "other-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new DeleteFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new DeleteFeatureFlagCommand("other-user", 1, 1, 1);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.HandleAsync(command));
    }
}
