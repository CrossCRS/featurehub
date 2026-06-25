using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using FeatureHub.Application.Common.Interfaces.Authorization;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace FeatureHub.Application.FeatureFlags.Commands.CreateFeatureFlag;

public class CreateFeatureFlagCommand : Command
{
    public string UserId { get; }
    public int ProjectId { get; }
    public int EnvironmentId { get; }
    public string Name { get; }
    public string? Description { get; }
    public bool Value { get; }
    public string? Data { get; }
    public int AddedFeatureFlagId { get; set; }

    public CreateFeatureFlagCommand(string userId, int projectId, int environmentId, string name, string? description, bool value, string? data) : base(Guid.NewGuid())
    {
        UserId = userId;
        ProjectId = projectId;
        EnvironmentId = environmentId;
        Name = name;
        Description = description;
        Value = value;
        Data = data;
    }
}

public class CreateFeatureFlagCommandHandler : RequestHandlerAsync<CreateFeatureFlagCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectAuthorization _projectAuthorization;

    public CreateFeatureFlagCommandHandler(IApplicationDbContext context, IProjectAuthorization projectAuthorization)
    {
        _context = context;
        _projectAuthorization = projectAuthorization;
    }

    [ValidateRequest(step: 1)]
    public override async Task<CreateFeatureFlagCommand> HandleAsync(CreateFeatureFlagCommand command, CancellationToken cancellationToken = default)
    {
        if (!await _projectAuthorization.UserCanModifyProjectAsync(command.ProjectId, command.UserId, cancellationToken))
        {
            throw new ForbiddenAccessException("You do not have permission to modify this feature flag.");
        }

        var environment = await _context.Environments
            .SingleOrDefaultAsync(e => e.Id == command.EnvironmentId && e.ProjectId == command.ProjectId, cancellationToken);

        if (environment == null || environment.ProjectId != command.ProjectId)
        {
            throw new NotFoundException(nameof(Domain.Entities.Environment), command.EnvironmentId);
        }

        var featureFlag = new Domain.Entities.FeatureFlag
        {
            EnvironmentId = command.EnvironmentId,
            Name = command.Name,
            Description = command.Description,
            Value = command.Value,
            Data = command.Data
        };

        _context.FeatureFlags.Add(featureFlag);
        await _context.SaveChangesAsync(cancellationToken);

        command.AddedFeatureFlagId = featureFlag.Id;

        return await base.HandleAsync(command, cancellationToken);
    }
}
