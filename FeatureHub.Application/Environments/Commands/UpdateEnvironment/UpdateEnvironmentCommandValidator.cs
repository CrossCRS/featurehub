using FluentValidation;

namespace FeatureHub.Application.Environments.Commands.UpdateEnvironment;

public class UpdateEnvironmentCommandValidator : AbstractValidator<UpdateEnvironmentCommand>
{
    public UpdateEnvironmentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("User ID must be a valid GUID.");
        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");
        RuleFor(x => x.EnvironmentId)
            .GreaterThan(0).WithMessage("Environment ID must be greater than 0.");
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Environment name must not exceed 100 characters.");
        RuleFor(x => x.Token)
            .MaximumLength(65).WithMessage("Token must not exceed 65 characters.");
    }
}
