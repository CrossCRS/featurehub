using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using FeatureHub.Application.FeatureFlags.Commands.UpdateFeatureFlag;
using FeatureHub.Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.FeatureFlags.Commands;

public class UpdateFeatureFlagCommandTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();
    private readonly Mock<IProjectAuthorization> _mockProjectAuthorization = new();

    [Fact]
    public async Task UpdateFeatureFlag_ShouldUpdateOnlyName_WhenFeatureFlagExistsAndUserHasAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var featureFlags = new List<FeatureFlag>
        {
            new() { Id = 1, EnvironmentId = 1, Name = "Old Flag", Value = true, IsActive = true, IsDeleted = false, Environment = environments[0] },
        };

        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new UpdateFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new UpdateFeatureFlagCommand("owner1", 1, 1, 1, "New Flag", null, null, null, null);

        await handler.HandleAsync(command);

        Assert.Equal("New Flag", featureFlags[0].Name);
        Assert.True(featureFlags[0].Value);
        Assert.True(featureFlags[0].IsActive);
        Assert.False(featureFlags[0].IsDeleted);
        Assert.Equal(environments[0], featureFlags[0].Environment);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFeatureFlag_ShouldUpdateAllProperties_WhenFeatureFlagExistsAndUserHasAccess()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var featureFlags = new List<FeatureFlag>
        {
            new() { Id = 1, EnvironmentId = 1, Name = "Old Flag", Description = "Old description", Data = "old-data", Value = true, IsActive = false, IsDeleted = false, Environment = environments[0] },
        };

        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new UpdateFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new UpdateFeatureFlagCommand("owner1", 1, 1, 1, "New Flag", "New description", false, "new-data", true);

        await handler.HandleAsync(command);

        Assert.Equal("New Flag", featureFlags[0].Name);
        Assert.Equal("New description", featureFlags[0].Description);
        Assert.Equal("new-data", featureFlags[0].Data);
        Assert.False(featureFlags[0].Value);
        Assert.True(featureFlags[0].IsActive);
        Assert.False(featureFlags[0].IsDeleted);
        Assert.Equal(environments[0], featureFlags[0].Environment);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFeatureFlag_ShouldNullOutDescription_WhenPassedEmptyString()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var featureFlags = new List<FeatureFlag>
        {
            new() { Id = 1, EnvironmentId = 1, Name = "Old Flag", Description = "Old description", Value = true, IsActive = true, IsDeleted = false, Environment = environments[0] },
        };

        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new UpdateFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new UpdateFeatureFlagCommand("owner1", 1, 1, 1, null, "", null, null, null);

        await handler.HandleAsync(command);

        Assert.Null(featureFlags[0].Description);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFeatureFlag_ShouldNullOutData_WhenPassedEmptyString()
    {
        var environments = new List<FeatureHub.Domain.Entities.Environment>
        {
            new() { Id = 1, ProjectId = 1, Name = "Production", Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var featureFlags = new List<FeatureFlag>
        {
            new() { Id = 1, EnvironmentId = 1, Name = "Old Flag", Data = "some-data-here", Value = true, IsActive = true, IsDeleted = false, Environment = environments[0] },
        };

        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);
        _mockProjectAuthorization.Setup(a => a.UserCanModifyProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new UpdateFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new UpdateFeatureFlagCommand("owner1", 1, 1, 1, null, null, null, "", null);

        await handler.HandleAsync(command);

        Assert.Null(featureFlags[0].Data);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFeatureFlag_ShouldThrowNotFoundException_WhenFeatureFlagDoesNotExist()
    {
        var featureFlags = new List<FeatureFlag>();

        _mockContext.Setup(c => c.FeatureFlags).Returns(featureFlags.BuildMockDbSet().Object);

        var handler = new UpdateFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new UpdateFeatureFlagCommand("owner1", 1, 1, 999, "Name", null, null, null, null);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command));
    }

    [Fact]
    public async Task UpdateFeatureFlag_ShouldThrowForbiddenAccessException_WhenUserHasNoAccess()
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

        var handler = new UpdateFeatureFlagCommandHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var command = new UpdateFeatureFlagCommand("other-user", 1, 1, 1, "Name", null, null, null, null);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.HandleAsync(command));
    }
}
