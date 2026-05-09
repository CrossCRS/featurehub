using FeatureHub.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FeatureHub.Infrastructure.Identity;

internal class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User is not authenticated.");
    }

    public string GetUserName()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.Name
            ?? throw new InvalidOperationException("User is not authenticated.");
    }
}
