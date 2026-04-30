using FeatureHub.Application.Common.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Paramore.Brighter;

namespace FeatureHub.Application.Common.Middleware;

public class ValidationHandler<TRequest> : RequestHandlerAsync<TRequest> where TRequest : class, IRequest
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationHandler(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public override async Task<TRequest> HandleAsync(TRequest command, CancellationToken cancellationToken = default)
    {
        var validationResults = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(command, cancellationToken);

            if (!result.IsValid)
            {
                validationResults.AddRange(result.Errors);
            }
        }

        if (validationResults.Count > 0)
        {
            throw new ValidationAppException(validationResults);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
