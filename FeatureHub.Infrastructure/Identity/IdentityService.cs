using FeatureHub.Application.Common.Interfaces.Identity;
using FeatureHub.Application.Common.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FeatureHub.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService<ApplicationUser> _tokenService;
    private readonly IConfiguration _configuration;

    public IdentityService(UserManager<ApplicationUser> userManager, IJwtTokenService<ApplicationUser> tokenService, IConfiguration configuration)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    private int GetRefreshTokenExpirationMinutes()
    {
        return _configuration.GetSection("JwtSettings").GetValue<int>("RefreshTokenExpirationMinutes", 10080);
    }

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
            return null;

        if (!await _userManager.CheckPasswordAsync(user, password))
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddMinutes(GetRefreshTokenExpirationMinutes());
        await _userManager.UpdateAsync(user);

        return new LoginResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<LoginResponse?> RefreshTokenAsync(string expiredAccessToken, string refreshToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(expiredAccessToken);

        if (principal == null || principal.Identity == null)
            return null;

        var user = await _userManager.FindByNameAsync(principal.Identity.Name!);

        if (user == null)
            return null;

        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _tokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddMinutes(GetRefreshTokenExpirationMinutes());
        await _userManager.UpdateAsync(user);

        return new LoginResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
    }
}
