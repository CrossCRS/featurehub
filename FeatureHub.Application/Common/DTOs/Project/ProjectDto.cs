namespace FeatureHub.Application.Common.DTOs.Project;

public class ProjectDto
{
    public int Id { get; set; }
    public required string OwnerId { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
