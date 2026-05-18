using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.DTOs.Environment;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;

namespace FeatureHub.Application.Environments.Queries.GetEnvironmentById;

public class GetEnvironmentById : IQuery<EnvironmentDto>
{
    public string UserId { get; }
    public int ProjectId { get; }
    public int EnvironmentId { get; }

    public GetEnvironmentById(string userId, int projectId, int environmentId)
    {
        UserId = userId;
        ProjectId = projectId;
        EnvironmentId = environmentId;
    }
}

public class GetEnvironmentByIdHandler : QueryHandlerAsync<GetEnvironmentById, EnvironmentDto>
{
    private readonly IApplicationDbContext _context;

    public GetEnvironmentByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<EnvironmentDto> ExecuteAsync(GetEnvironmentById query, CancellationToken cancellationToken)
    {
        var environment = await _context.Environments
            .Where(e => e.Id == query.EnvironmentId)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (environment == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Environment), query.EnvironmentId);
        }

        if (environment.ProjectId != query.ProjectId)
        {
            throw new NotFoundException(nameof(Domain.Entities.Environment), query.EnvironmentId);
        }

        var ownsProject = await _context.Projects
            .AnyAsync(p => p.Id == query.ProjectId && p.OwnerId == query.UserId, cancellationToken);

        if (!ownsProject)
        {
            throw new ForbiddenAccessException("You do not have access to this environment.");
        }

        return environment;
    }
}
