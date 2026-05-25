using FluentValidation;

namespace FeatureHub.Application.FeatureFlags.Commands.DeleteFeatureFlag;

public class DeleteFeatureFlagCommandValidator : AbstractValidator<DeleteFeatureFlagCommand>
{
    public DeleteFeatureFlagCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");

        RuleFor(x => x.EnviromentId)
            .GreaterThan(0).WithMessage("Environment ID must be greater than 0.");

        RuleFor(x => x.FeatureFlagId)
            .GreaterThan(0).WithMessage("Feature flag ID must be greater than 0.");
    }
}
