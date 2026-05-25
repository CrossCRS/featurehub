using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Authorization;
using FeatureHub.Application.Common.DTOs.FeatureFlag;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;

namespace FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagById;

public class GetFeatureFlagById : IQuery<FeatureFlagDto>
{
    public string UserId { get; }
    public int ProjectId { get; }
    public int EnvironmentId { get; }
    public int FeatureFlagId { get; }

    public GetFeatureFlagById(string userId, int projectId, int environmentId, int featureFlagId)
    {
        UserId = userId;
        ProjectId = projectId;
        EnvironmentId = environmentId;
        FeatureFlagId = featureFlagId;
    }
}

public class GetFeatureFlagByIdHandler : QueryHandlerAsync<GetFeatureFlagById, FeatureFlagDto>
{
    private readonly IApplicationDbContext _context;

    public GetFeatureFlagByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<FeatureFlagDto> ExecuteAsync(GetFeatureFlagById query, CancellationToken cancellationToken)
    {
        if (!await ProjectAuthorization.UserCanAccessProjectAsync(_context, query.ProjectId, query.UserId, cancellationToken))
        {
            throw new ForbiddenAccessException("You do not have access to this feature flag.");
        }

        var featureFlag = await _context.FeatureFlags
            .Where(ff => ff.Id == query.FeatureFlagId && ff.EnvironmentId == query.EnvironmentId && ff.Environment!.ProjectId == query.ProjectId)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (featureFlag == null)
        {
            throw new NotFoundException(nameof(FeatureFlag), query.FeatureFlagId);
        }

        return featureFlag;
    }
}
