using FeatureHub.Domain.Common;

namespace FeatureHub.Domain.Entities;

public class FeatureFlagRule : BaseAuditableEntity
{
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public required string Type { get; set; }
    public int Priority { get; set; }
    public string? Config { get; set; }

    public required int FeatureFlagId { get; set; }
    public FeatureFlag? FeatureFlag { get; set; }
}