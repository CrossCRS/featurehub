using FluentValidation;

namespace FeatureHub.Application.Environments.Commands.CreateEnvironment;

public class CreateEnvironmentCommandValidator : AbstractValidator<CreateEnvironmentCommand>
{
    public CreateEnvironmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Environment name is required.")
            .MaximumLength(100).WithMessage("Environment name must not exceed 100 characters.");
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.")
            .MaximumLength(65).WithMessage("Token must not exceed 65 characters.");
        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");
    }
}
