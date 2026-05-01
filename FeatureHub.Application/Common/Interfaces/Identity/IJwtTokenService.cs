using System.Security.Claims;

namespace FeatureHub.Application.Common.Interfaces.Identity;

public interface IJwtTokenService<TUser> where TUser : class
{
    string GenerateAccessToken(TUser user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}