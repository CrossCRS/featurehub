namespace FeatureHub.Application.Common.DTOs.FeatureFlag;

public class FeatureFlagDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool Value { get; set; }
    public string? Data { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public int EnvironmentId { get; set; }
}
