using System.Net;

namespace FeatureHub.Application.Common.Exceptions;

public class ForbiddenAccessException : AppException
{
    public ForbiddenAccessException(string message = "You do not have permission to perform this action.") : base(message, HttpStatusCode.Forbidden)
    {
    }
}
