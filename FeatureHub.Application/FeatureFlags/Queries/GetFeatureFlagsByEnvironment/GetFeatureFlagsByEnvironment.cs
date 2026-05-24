using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Authorization;
using FeatureHub.Application.Common.DTOs.FeatureFlag;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;

namespace FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagsByEnvironment;

public class GetFeatureFlagsByEnvironment : IQuery<IEnumerable<FeatureFlagDto>>
{
    public string UserId { get; }
    public int ProjectId { get; }
    public int EnvironmentId { get; }

    public GetFeatureFlagsByEnvironment(string userId, int projectId, int environmentId)
    {
        UserId = userId;
        ProjectId = projectId;
        EnvironmentId = environmentId;
    }
}

public class GetFeatureFlagsByEnvironmentHandler : QueryHandlerAsync<GetFeatureFlagsByEnvironment, IEnumerable<FeatureFlagDto>>
{
    private readonly IApplicationDbContext _context;

    public GetFeatureFlagsByEnvironmentHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<IEnumerable<FeatureFlagDto>> ExecuteAsync(GetFeatureFlagsByEnvironment query, CancellationToken cancellationToken)
    {
        if (!await ProjectAuthorization.UserCanAccessProjectAsync(_context, query.ProjectId, query.UserId, cancellationToken))
        {
            throw new ForbiddenAccessException("You do not have access to this project.");
        }

        var featureFlags = await _context.FeatureFlags
            .Where(ff => ff.EnvironmentId == query.EnvironmentId && ff.Environment!.ProjectId == query.ProjectId)
            .AsNoTracking()
            .Select(ff => new FeatureFlagDto
            {
                Id = ff.Id,
                Name = ff.Name,
                Description = ff.Description,
                Value = ff.Value,
                Data = ff.Data,
                IsActive = ff.IsActive,
                CreatedAt = ff.CreatedAt,
                UpdatedAt = ff.UpdatedAt,
                EnvironmentId = ff.EnvironmentId
            })
            .ToListAsync(cancellationToken);

        return featureFlags;
    }
}