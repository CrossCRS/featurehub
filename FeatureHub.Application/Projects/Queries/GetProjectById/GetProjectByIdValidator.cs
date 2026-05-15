using FluentValidation;

namespace FeatureHub.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdValidator : AbstractValidator<GetProjectById>
{
    public GetProjectByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Project ID must be greater than 0.");
    }
}
