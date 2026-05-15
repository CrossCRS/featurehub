using System.Net;

namespace FeatureHub.Application.Common.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string name, object key) : base($"\"{name}\" ({key}) was not found.", HttpStatusCode.NotFound)
    {
    }
}
