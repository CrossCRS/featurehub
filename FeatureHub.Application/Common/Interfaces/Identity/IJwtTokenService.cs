namespace FeatureHub.Application.Common.Interfaces.Identity;

public interface IJwtTokenService<TUser> where TUser : class
{
    string GenerateToken(TUser user, IEnumerable<string> roles);
}