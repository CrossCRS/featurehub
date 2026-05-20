using FeatureHub.Domain.Common;

namespace FeatureHub.Domain.Entities;

public class FeatureFlag : BaseAuditableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool Value { get; set; }
    public string? Data { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public required int EnvironmentId { get; set; }
    public Environment? Environment { get; set; }
}
