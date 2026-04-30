using FluentValidation.Results;
using System.Net;

namespace FeatureHub.Application.Common.Exceptions;

public class ValidationAppException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationAppException(IEnumerable<ValidationFailure> errors) : base("Validation failed.", HttpStatusCode.BadRequest)
    {
        Errors = errors
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}
