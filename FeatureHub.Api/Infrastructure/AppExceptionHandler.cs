using FeatureHub.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FeatureHub.Api.Infrastructure;

public class AppExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AppExceptionHandler(IProblemDetailsService problemDetailsService, IWebHostEnvironment webHostEnvironment)
    {
        _problemDetailsService = problemDetailsService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationAppException e => new ValidationProblemDetails(e.Errors)
            {
                Status = (int?)e.StatusCode,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            },
            //NotFoundException ne => (StatusCodes.Status404NotFound, new ProblemDetails
            //{
            //    Status = StatusCodes.Status404NotFound,
            //    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            //    Title = "The specified resource was not found.",
            //    Detail = ne.Message
            //}),
            InvalidCredentialsException e => new ProblemDetails
            {
                Status = (int?)e.StatusCode,
                Title = e.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2"
            },
            //ForbiddenAccessException => (new ProblemDetails
            //{
            //    Status = StatusCodes.Status403Forbidden,
            //    Title = "Forbidden",
            //    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4"
            //}),
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            }
        };

        // Add details if in development environment
        if (_webHostEnvironment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = new
            {
                exception.Message,
                exception.StackTrace,
                InnerException = exception.InnerException != null ? new
                {
                    exception.InnerException.Message,
                    exception.InnerException.StackTrace
                } : null
            };
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}
