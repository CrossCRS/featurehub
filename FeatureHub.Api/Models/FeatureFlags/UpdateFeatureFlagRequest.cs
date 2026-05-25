namespace FeatureHub.Api.Models.FeatureFlags;

public class UpdateFeatureFlagRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? Value { get; set; }
    public string? Data { get; set; }
    public bool? IsActive { get; set; }
}
