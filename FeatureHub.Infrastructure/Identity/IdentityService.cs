using FeatureHub.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Identity;

namespace FeatureHub.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService<ApplicationUser> _tokenService;

    public IdentityService(UserManager<ApplicationUser> userManager, IJwtTokenService<ApplicationUser> tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
            return null;

        if (!await _userManager.CheckPasswordAsync(user, password))
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return token;
    }
}
