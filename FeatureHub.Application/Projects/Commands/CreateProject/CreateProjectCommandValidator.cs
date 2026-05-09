using FluentValidation;

namespace FeatureHub.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MaximumLength(100).WithMessage("Project name must not exceed 100 characters.");
        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Owner ID is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("Owner ID must be a valid GUID.");
    }
}
