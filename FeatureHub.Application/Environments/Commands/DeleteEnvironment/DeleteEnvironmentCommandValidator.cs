using FluentValidation;

namespace FeatureHub.Application.Environments.Commands.DeleteEnvironment;

public class DeleteEnvironmentCommandValidator : AbstractValidator<DeleteEnvironmentCommand>
{
    public DeleteEnvironmentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("User ID must be a valid GUID.");
        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");
        RuleFor(x => x.EnvironmentId)
            .GreaterThan(0).WithMessage("Environment ID must be greater than 0.");
    }
}
