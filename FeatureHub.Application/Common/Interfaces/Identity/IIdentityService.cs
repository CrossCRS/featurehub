using FeatureHub.Application.Common.Models.Identity;

namespace FeatureHub.Application.Common.Interfaces.Identity;

public interface IIdentityService
{
    Task<LoginResponse?> LoginAsync(string username, string password);
    Task<LoginResponse?> RefreshTokenAsync(string expiredAccessToken, string refreshToken);
}
