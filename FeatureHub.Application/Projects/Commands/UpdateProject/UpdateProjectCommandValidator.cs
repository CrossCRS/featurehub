using FluentValidation;

namespace FeatureHub.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("User ID must be a valid GUID.");
        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Project name must not exceed 100 characters.");
    }
}
