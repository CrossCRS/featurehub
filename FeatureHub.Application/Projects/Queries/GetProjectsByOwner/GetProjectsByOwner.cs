using FeatureHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;

namespace FeatureHub.Application.Projects.Queries.GetProjectsByOwner;

public class GetProjectsByOwner : IQuery<IEnumerable<ProjectDto>>
{
    public string OwnerId { get; }

    public GetProjectsByOwner(string ownerId)
    {
        OwnerId = ownerId;
    }
}

public class GetProjectsByOwnerHandler : QueryHandlerAsync<GetProjectsByOwner, IEnumerable<ProjectDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProjectsByOwnerHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task<IEnumerable<ProjectDto>> ExecuteAsync(GetProjectsByOwner query, CancellationToken cancellationToken)
    {
        var projects = await _context.Projects
            .Where(p => p.OwnerId == query.OwnerId && !p.IsDeleted)
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
            .ToListAsync(cancellationToken);

        return projects;
    }
}
