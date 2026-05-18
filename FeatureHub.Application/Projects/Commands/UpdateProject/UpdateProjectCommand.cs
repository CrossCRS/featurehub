using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace FeatureHub.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommand : Command
{
    public string UserId { get; }
    public int ProjectId { get; }

    public string? Name { get; }
    public bool? IsActive { get; }

    public UpdateProjectCommand(string userId, int projectId, string? name, bool? isActive) : base(Guid.NewGuid())
    {
        UserId = userId;
        ProjectId = projectId;
        Name = name;
        IsActive = isActive;
    }
}

public class UpdateProjectCommandHandler : RequestHandlerAsync<UpdateProjectCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<UpdateProjectCommand> HandleAsync(UpdateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects.SingleOrDefaultAsync(p => p.Id == command.ProjectId && !p.IsDeleted, cancellationToken);

        if (project == null)
        {
            throw new NotFoundException(nameof(Project), command.ProjectId);
        }

        if (project.OwnerId != command.UserId)
        {
            throw new ForbiddenAccessException("You do not have permission to delete this project.");
        }

        if (!string.IsNullOrEmpty(command.Name))
        {
            project.Name = command.Name;
        }

        if (command.IsActive.HasValue)
        {
            project.IsActive = command.IsActive.Value;
        }

        project.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}