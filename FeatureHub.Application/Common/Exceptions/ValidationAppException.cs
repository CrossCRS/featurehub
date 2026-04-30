using FluentValidation.Results;
using System.Net;

namespace FeatureHub.Application.Common.Exceptions;

public class ValidationAppException : AppException
{
    public IList<ValidationFailure> Errors { get; }

    public ValidationAppException(IList<ValidationFailure> errors) : base("Validation failed.", HttpStatusCode.BadRequest)
    {
        Errors = errors;
    }
}
