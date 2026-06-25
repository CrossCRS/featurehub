using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.DTOs.Environment;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;

namespace FeatureHub.Application.Environments.Queries.GetEnvironmentsByProject;

public class GetEnvironmentsByProject : IQuery<IEnumerable<EnvironmentDto>>
{
    public string UserId { get; }
    public int ProjectId { get; }

    public GetEnvironmentsByProject(string userId, int projectId)
    {
        UserId = userId;
        ProjectId = projectId;
    }
}

public class GetEnvironmentsByProjectHandler : QueryHandlerAsync<GetEnvironmentsByProject, IEnumerable<EnvironmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectAuthorization _projectAuthorization;

    public GetEnvironmentsByProjectHandler(IApplicationDbContext context, IProjectAuthorization projectAuthorization)
    {
        _context = context;
        _projectAuthorization = projectAuthorization;
    }

    [ValidateRequest(step: 1)]
    public override async Task<IEnumerable<EnvironmentDto>> ExecuteAsync(GetEnvironmentsByProject query, CancellationToken cancellationToken)
    {
        if (!await _projectAuthorization.UserCanAccessProjectAsync(query.ProjectId, query.UserId, cancellationToken))
        {
            throw new ForbiddenAccessException("You do not have access to this project.");
        }

        var environments = await _context.Environments
            .Where(e => e.ProjectId == query.ProjectId)
            .AsNoTracking()
            .Select(e => new EnvironmentDto
            {
                Id = e.Id,
                ProjectId = e.ProjectId,
                Name = e.Name,
                Token = e.Token,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return environments;
    }
}
