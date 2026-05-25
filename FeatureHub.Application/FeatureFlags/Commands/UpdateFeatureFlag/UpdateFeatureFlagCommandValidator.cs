using FluentValidation;

namespace FeatureHub.Application.FeatureFlags.Commands.UpdateFeatureFlag;

public class UpdateFeatureFlagCommandValidator : AbstractValidator<UpdateFeatureFlagCommand>
{
    public UpdateFeatureFlagCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");

        RuleFor(x => x.EnvironmentId)
            .GreaterThan(0).WithMessage("Environment ID must be greater than 0.");

        RuleFor(x => x.FeatureFlagId)
            .GreaterThan(0).WithMessage("Feature flag ID must be greater than 0.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Feature flag name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Feature flag description must not exceed 500 characters.");

        RuleFor(x => x.Data)
            .MaximumLength(1000).WithMessage("Feature flag data must not exceed 1000 characters.");
    }
}
