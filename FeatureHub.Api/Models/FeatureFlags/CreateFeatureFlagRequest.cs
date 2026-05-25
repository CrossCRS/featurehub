namespace FeatureHub.Api.Models.FeatureFlags;

public class CreateFeatureFlagRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool Value { get; set; } = false;
    public string? Data { get; set; }
}
