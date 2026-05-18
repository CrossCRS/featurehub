using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace FeatureHub.Application.Environments.Commands.UpdateEnvironment;

public class UpdateEnvironmentCommand : Command
{
    public string UserId { get; }
    public int ProjectId { get; }
    public int EnvironmentId { get; }
    public string? Name { get; }
    public bool? IsActive { get; }
    public string? Token { get; }

    public UpdateEnvironmentCommand(string userId, int projectId, int environmentId, string? name, bool? isActive, string? token) : base(Guid.NewGuid())
    {
        UserId = userId;
        ProjectId = projectId;
        EnvironmentId = environmentId;
        Name = name;
        IsActive = isActive;
        Token = token;
    }
}

public class UpdateEnvironmentCommandHandler : RequestHandlerAsync<UpdateEnvironmentCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateEnvironmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<UpdateEnvironmentCommand> HandleAsync(UpdateEnvironmentCommand command, CancellationToken cancellationToken = default)
    {
        var environment = await _context.Environments
            .Include(e => e.Project)
            .SingleOrDefaultAsync(e => e.Id == command.EnvironmentId, cancellationToken);

        if (environment == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Environment), command.EnvironmentId);
        }

        if (environment.Project!.OwnerId != command.UserId)
        {
            throw new ForbiddenAccessException("You do not have permission to update this environment.");
        }

        if (!string.IsNullOrEmpty(command.Name))
        {
            environment.Name = command.Name;
        }

        if (command.IsActive.HasValue)
        {
            environment.IsActive = command.IsActive.Value;
        }

        if (!string.IsNullOrEmpty(command.Token))
        {
            environment.Token = command.Token;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
