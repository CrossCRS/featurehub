using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FeatureHub.Application.Common.Authorization;

public class ProjectAuthorization : IProjectAuthorization
{
    private readonly IApplicationDbContext _context;

    public ProjectAuthorization(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserCanAccessProjectAsync(int projectId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AnyAsync(p => p.Id == projectId && p.OwnerId == userId, cancellationToken);
    }

    public async Task<bool> UserCanModifyProjectAsync(int projectId, string userId, CancellationToken cancellationToken = default)
    {
        // For now, only the project owner can modify the project.
        // Could add permissions later.
        return await UserCanAccessProjectAsync(projectId, userId, cancellationToken);
    }
}
