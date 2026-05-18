using FeatureHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FeatureHub.Application.Common.Authorization;

public static class ProjectAuthorization
{
    public static async Task<bool> UserCanAccessProjectAsync(IApplicationDbContext context, int projectId, string userId, CancellationToken cancellationToken = default)
    {
        return await context.Projects
            .AnyAsync(p => p.Id == projectId && p.OwnerId == userId, cancellationToken);
    }
}
