using FluentValidation;

namespace FeatureHub.Application.Environments.Queries.GetEnvironmentsByProject;

public class GetEnvironmentsByProjectValidator : AbstractValidator<GetEnvironmentsByProject>
{
    public GetEnvironmentsByProjectValidator()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");
    }
}
