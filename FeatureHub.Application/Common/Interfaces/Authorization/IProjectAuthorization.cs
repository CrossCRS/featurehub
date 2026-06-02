namespace FeatureHub.Application.Common.Interfaces.Authorization;

public interface IProjectAuthorization
{
    Task<bool> UserCanAccessProjectAsync(int projectId, string userId, CancellationToken cancellationToken = default);
    Task<bool> UserCanModifyProjectAsync(int projectId, string userId, CancellationToken cancellationToken = default);
}
