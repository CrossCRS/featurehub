namespace FeatureHub.Application.Common.DTOs.FeatureFlag;

public class PublicFeatureFlagDto
{
    public required string Name { get; set; }
    public bool Value { get; set; }
    public string? Data { get; set; }
}
