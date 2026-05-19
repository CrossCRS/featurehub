using FluentValidation;

namespace FeatureHub.Application.Environments.Queries.GetEnvironmentById;

public class GetEnvironmentByIdValidator : AbstractValidator<GetEnvironmentById>
{
    public GetEnvironmentByIdValidator()
    {
        RuleFor(x => x.EnvironmentId)
            .GreaterThan(0).WithMessage("Environment ID must be greater than 0.");
    }
}
