using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.DTOs.Project;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;

namespace FeatureHub.Application.Projects.Queries.GetProjectById;

public class GetProjectById : IQuery<ProjectDto>
{
    public string UserId { get; }
    public int Id { get; }

    public GetProjectById(string userId, int id)
    {
        UserId = userId;
        Id = id;
    }
}

public class GetProjectByIdHandler : QueryHandlerAsync<GetProjectById, ProjectDto>
{
    private readonly IApplicationDbContext _context;

    public GetProjectByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<ProjectDto> ExecuteAsync(GetProjectById query, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .Where(p => p.Id == query.Id)
            .AsNoTracking()
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                OwnerId = p.OwnerId,
                Name = p.Name,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (project == null)
        {
            throw new NotFoundException(nameof(Project), query.Id);
        }

        if (project.OwnerId != query.UserId)
        {
            // TODO: Consider returning a 404 to avoid revealing the existence of the project?
            throw new ForbiddenAccessException("You do not have access to this project.");
        }

        return project;
    }
}