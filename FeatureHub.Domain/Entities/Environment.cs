using FeatureHub.Domain.Common;

namespace FeatureHub.Domain.Entities;

public class Environment : BaseAuditableEntity
{
    public required string Name { get; set; }
    public required string Token { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public required int ProjectId { get; set; }
    public Project? Project { get; set; }

    public ICollection<FeatureFlag> FeatureFlags { get; set; } = [];
}
