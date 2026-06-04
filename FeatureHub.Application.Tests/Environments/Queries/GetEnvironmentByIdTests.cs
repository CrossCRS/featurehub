using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using FeatureHub.Application.Environments.Queries.GetEnvironmentById;
using MockQueryable.Moq;
using Moq;

namespace FeatureHub.Application.Tests.Environments.Queries;

public class GetEnvironmentByIdTests
{
    private readonly Mock<IApplicationDbContext> _mockContext = new();
    private readonly Mock<IProjectAuthorization> _mockProjectAuthorization = new();

    [Fact]
    public async Task GetEnvironmentById_ShouldReturnEnvironment_WhenEnvironmentExistsAndUserHasAccess()
    {
        var environments = new List<Domain.Entities.Environment>
        {
            new() { Id = 1, Name = "Environment 1", ProjectId = 1, Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new GetEnvironmentByIdHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetEnvironmentById("owner1", 1, 1);

        var result = await handler.ExecuteAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Environment 1", result.Name);
    }

    [Fact]
    public async Task GetEnvironmentById_ShouldThrowNotFoundException_WhenEnvironmentDoesNotExist()
    {
        var environments = new List<Domain.Entities.Environment>();
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);

        var handler = new GetEnvironmentByIdHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetEnvironmentById("owner1", 1, 999);

        await Assert.ThrowsAsync<FeatureHub.Application.Common.Exceptions.NotFoundException>(() => handler.ExecuteAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetEnvironmentById_ShouldThrowForbiddenAccessException_WhenUserHasNoAccess()
    {
        var environments = new List<Domain.Entities.Environment>
        {
            new() { Id = 1, Name = "Environment 1", ProjectId = 1, Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(1, "other-user", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new GetEnvironmentByIdHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetEnvironmentById("other-user", 1, 1);

        await Assert.ThrowsAsync<FeatureHub.Application.Common.Exceptions.ForbiddenAccessException>(() => handler.ExecuteAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetEnvironmentById_ShouldThrowNotFoundException_WhenEnvironmentDoesNotBelongToProject()
    {
        var environments = new List<Domain.Entities.Environment>
        {
            new() { Id = 1, Name = "Environment 1", ProjectId = 1, Token = "deadbeef000000000000000000000042", IsActive = true, IsDeleted = false },
        };
        var mockDbSet = environments.BuildMockDbSet();
        _mockContext.Setup(c => c.Environments).Returns(mockDbSet.Object);
        _mockProjectAuthorization.Setup(a => a.UserCanAccessProjectAsync(2, "owner1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new GetEnvironmentByIdHandler(_mockContext.Object, _mockProjectAuthorization.Object);
        var query = new GetEnvironmentById("owner1", 2, 1);

        await Assert.ThrowsAsync<FeatureHub.Application.Common.Exceptions.NotFoundException>(() => handler.ExecuteAsync(query, CancellationToken.None));
    }
}
