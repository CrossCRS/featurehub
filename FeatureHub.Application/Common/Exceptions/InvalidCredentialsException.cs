using System.Net;

namespace FeatureHub.Application.Common.Exceptions;

public class InvalidCredentialsException : AppException
{
    public InvalidCredentialsException(string message = "Invalid username or password.") : base(message, HttpStatusCode.Unauthorized)
    {
    }
}
