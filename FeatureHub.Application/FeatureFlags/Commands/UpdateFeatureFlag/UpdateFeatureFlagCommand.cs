using FeatureHub.Application.Common.Attributes;
using FeatureHub.Application.Common.Authorization;
using FeatureHub.Application.Common.Exceptions;
using FeatureHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace FeatureHub.Application.FeatureFlags.Commands.UpdateFeatureFlag;

public class UpdateFeatureFlagCommand : Command
{
    public string UserId { get; }
    public int ProjectId { get; }
    public int EnvironmentId { get; }
    public int FeatureFlagId { get; }
    public string? Name { get; }
    public string? Description { get; }
    public bool? Value { get; }
    public string? Data { get; }
    public bool? IsActive { get; }

    public UpdateFeatureFlagCommand(string userId, int projectId, int environmentId, int featureFlagId, string? name, string? description, bool? value, string? data, bool? isActive) : base(Guid.NewGuid())
    {
        UserId = userId;
        ProjectId = projectId;
        EnvironmentId = environmentId;
        FeatureFlagId = featureFlagId;
        Name = name;
        Description = description;
        Value = value;
        Data = data;
        IsActive = isActive;
    }
}

public class UpdateFeatureFlagCommandHandler : RequestHandlerAsync<UpdateFeatureFlagCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateFeatureFlagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    [ValidateRequest(step: 1)]
    public override async Task<UpdateFeatureFlagCommand> HandleAsync(UpdateFeatureFlagCommand command, CancellationToken cancellationToken = default)
    {
        var featureFlag = await _context.FeatureFlags
            .Include(ff => ff.Environment)
            .SingleOrDefaultAsync(ff => ff.Id == command.FeatureFlagId && ff.EnvironmentId == command.EnvironmentId, cancellationToken);

        if (featureFlag == null || featureFlag.Environment!.ProjectId != command.ProjectId)
        {
            throw new NotFoundException(nameof(Domain.Entities.FeatureFlag), command.FeatureFlagId);
        }

        if (!await ProjectAuthorization.UserCanModifyProjectAsync(_context, featureFlag.Environment!.ProjectId, command.UserId, cancellationToken))
        {
            throw new ForbiddenAccessException("You do not have permission to update this feature flag.");
        }

        if (!string.IsNullOrEmpty(command.Name))
        {
            featureFlag.Name = command.Name;
        }

        if (command.Description != null)
        {
            featureFlag.Description = command.Description.Length == 0 ? null : command.Description;
        }

        if (command.Value.HasValue)
        {
            featureFlag.Value = command.Value.Value;
        }

        if (command.Data != null)
        {
            featureFlag.Data = command.Data.Length == 0 ? null : command.Data;
        }

        if (command.IsActive.HasValue)
        {
            featureFlag.IsActive = command.IsActive.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
