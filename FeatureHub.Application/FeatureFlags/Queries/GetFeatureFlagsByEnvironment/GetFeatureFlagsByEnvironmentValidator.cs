using FluentValidation;

namespace FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagsByEnvironment;

public class GetFeatureFlagsByEnvironmentValidator : AbstractValidator<GetFeatureFlagsByEnvironment>
{
    public GetFeatureFlagsByEnvironmentValidator()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");

        RuleFor(x => x.EnvironmentId)
            .GreaterThan(0).WithMessage("Environment ID must be greater than 0.");
    }
}
