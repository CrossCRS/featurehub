using System.Net;

namespace FeatureHub.Application.Common.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "You are not authorized to perform this action.") : base(message, HttpStatusCode.Unauthorized)
    {
    }
}
