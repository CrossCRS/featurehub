using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace FeatureHub.Application.Environments.Commands.RegenerateEnvironmentToken;

public class RegenerateEnvironmentTokenCommand : Command
{
    public string UserId { get; }
    public int ProjectId { get; }
    public int EnvironmentId { get; }
    public string NewToken { get; set; } = string.Empty;

    public RegenerateEnvironmentTokenCommand(string userId, int projectId, int environmentId) : base(Guid.NewGuid())
    {
        UserId = userId;
        ProjectId = projectId;
        EnvironmentId = environmentId;
    }
}

public class RegenerateEnvironmentTokenCommandHandler : RequestHandlerAsync<RegenerateEnvironmentTokenCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectAuthorization _projectAuthorization;

    public RegenerateEnvironmentTokenCommandHandler(IApplicationDbContext context, IProjectAuthorization projectAuthorization)
    {
        _context = context;
        _projectAuthorization = projectAuthorization;
    }

    [ValidateRequest(step: 1)]
    public override async Task<RegenerateEnvironmentTokenCommand> HandleAsync(RegenerateEnvironmentTokenCommand command, CancellationToken cancellationToken = default)
    {
        var environment = await _context.Environments
            .SingleOrDefaultAsync(e => e.Id == command.EnvironmentId, cancellationToken);

        if (environment == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Environment), command.EnvironmentId);
        }

        if (!await _projectAuthorization.UserCanModifyProjectAsync(environment.ProjectId, command.UserId, cancellationToken))
        {
            throw new ForbiddenAccessException("You do not have permission to refresh this environment's token.");
        }

        environment.Token = Guid.NewGuid().ToString("N");
        command.NewToken = environment.Token;

        await _context.SaveChangesAsync(cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
