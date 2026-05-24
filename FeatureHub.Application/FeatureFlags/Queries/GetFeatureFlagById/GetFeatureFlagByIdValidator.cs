using FluentValidation;

namespace FeatureHub.Application.FeatureFlags.Queries.GetFeatureFlagById;

public class GetFeatureFlagByIdValidator : AbstractValidator<GetFeatureFlagById>
{
    public GetFeatureFlagByIdValidator()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");

        RuleFor(x => x.EnvironmentId)
            .GreaterThan(0).WithMessage("Environment ID must be greater than 0.");

        RuleFor(x => x.FeatureFlagId)
            .GreaterThan(0).WithMessage("Feature Flag ID must be greater than 0.");
    }
}
