using FluentValidation;

namespace FeatureHub.Application.FeatureFlags.Queries.GetPublicFeatureFlags;

public class GetPublicFeatureFlagsValidator : AbstractValidator<GetPublicFeatureFlags>
{
    public GetPublicFeatureFlagsValidator()
    {
        RuleFor(x => x.EnvironmentToken)
            .NotEmpty().WithMessage("Environment token is required.")
            .MaximumLength(100).WithMessage("Environment token must not exceed 100 characters.");

        RuleFor(x => x.ClientHash)
            .MaximumLength(64).WithMessage("Client hash must not exceed 64 characters.");
    }
}
