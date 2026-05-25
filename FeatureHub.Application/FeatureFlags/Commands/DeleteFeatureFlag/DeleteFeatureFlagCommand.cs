using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Authorization;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace FeatureHub.Application.FeatureFlags.Commands.DeleteFeatureFlag;

public class DeleteFeatureFlagCommand : Command
{
    public string UserId { get; }
    public int ProjectId { get; }
    public int EnviromentId { get; }
    public int FeatureFlagId { get; }

    public DeleteFeatureFlagCommand(string userId, int projectId, int enviromentId, int featureFlagId) : base(Guid.NewGuid())
    {
        UserId = userId;
        ProjectId = projectId;
        EnviromentId = enviromentId;
        FeatureFlagId = featureFlagId;
    }
}

public class DeleteFeatureFlagCommandHandler : RequestHandlerAsync<DeleteFeatureFlagCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteFeatureFlagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<DeleteFeatureFlagCommand> HandleAsync(DeleteFeatureFlagCommand command, CancellationToken cancellationToken = default)
    {
        var featureFlag = await _context.FeatureFlags
            .Include(ff => ff.Environment)
            .SingleOrDefaultAsync(ff => ff.Id == command.FeatureFlagId && ff.EnvironmentId == command.EnviromentId, cancellationToken);

        if (featureFlag == null || featureFlag.Environment!.ProjectId != command.ProjectId)
        {
            throw new NotFoundException(nameof(Domain.Entities.FeatureFlag), command.FeatureFlagId);
        }

        if (!await ProjectAuthorization.UserCanModifyProjectAsync(_context, featureFlag.Environment!.ProjectId, command.UserId, cancellationToken))
        {
            throw new ForbiddenAccessException("You do not have permission to delete this feature flag.");
        }

        featureFlag.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
