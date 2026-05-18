using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Authorization;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace FeatureHub.Application.Environments.Commands.DeleteEnvironment;

public class DeleteEnvironmentCommand : Command
{
    public string UserId { get; }
    public int ProjectId { get; }
    public int EnvironmentId { get; }

    public DeleteEnvironmentCommand(string userId, int projectId, int environmentId) : base(Guid.NewGuid())
    {
        UserId = userId;
        ProjectId = projectId;
        EnvironmentId = environmentId;
    }
}

public class DeleteEnvironmentCommandHandler : RequestHandlerAsync<DeleteEnvironmentCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteEnvironmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<DeleteEnvironmentCommand> HandleAsync(DeleteEnvironmentCommand command, CancellationToken cancellationToken = default)
    {
        var environment = await _context.Environments
            .SingleOrDefaultAsync(e => e.Id == command.EnvironmentId, cancellationToken);

        if (environment == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Environment), command.EnvironmentId);
        }

        if (!await ProjectAuthorization.UserCanAccessProjectAsync(_context, environment.ProjectId, command.UserId, cancellationToken))
        {
            throw new ForbiddenAccessException("You do not have permission to delete this environment.");
        }

        environment.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
