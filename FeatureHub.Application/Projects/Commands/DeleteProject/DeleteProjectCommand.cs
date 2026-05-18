using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace FeatureHub.Application.Projects.Commands.DeleteProject;

public class DeleteProjectCommand : Command
{
    public string UserId { get; }
    public int ProjectId { get; }

    public DeleteProjectCommand(string userId, int projectId) : base(Guid.NewGuid())
    {
        UserId = userId;
        ProjectId = projectId;
    }
}

public class DeleteProjectCommandHandler : RequestHandlerAsync<DeleteProjectCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<DeleteProjectCommand> HandleAsync(DeleteProjectCommand command, CancellationToken cancellationToken = default)
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

        project.IsDeleted = true;
        project.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}