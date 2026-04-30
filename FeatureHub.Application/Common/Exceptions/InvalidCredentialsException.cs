using System.Net;

namespace FeatureHub.Application.Common.Exceptions;

public class InvalidCredentialsException : AppException
{
    public InvalidCredentialsException() : base("Invalid username or password.", HttpStatusCode.Unauthorized)
    {
    }
}
