using FeatureHub.Domain.Common;

namespace FeatureHub.Domain.Entities;

public class Project : BaseAuditableEntity
{
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public required string OwnerId { get; set; }
    public ICollection<Environment> Environments { get; set; } = [];
}
