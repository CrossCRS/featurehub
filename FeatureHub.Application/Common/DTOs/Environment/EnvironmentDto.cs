namespace FeatureHub.Application.Common.DTOs.Environment;

public class EnvironmentDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Token { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public int ProjectId { get; set; }
}
